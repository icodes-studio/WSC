const AWS = require('aws-sdk');
var moment = require('moment');
exports.handler = async (event, context) => {
    var docClient = new AWS.DynamoDB.DocumentClient();
    const inputObject = JSON.parse(event.body);

    //우선 해당 채팅방의 접속한 모든 유저를 가져온다.
    var params = {
        TableName: 'chatapp-userlist',
        IndexName: 'roomId-userId-index',
        KeyConditionExpression: '#HashKey = :hkey',
        ExpressionAttributeNames: { '#HashKey': 'roomId' },
        ExpressionAttributeValues: {
            ':hkey': inputObject.roomId
        }
    };
    const result = await docClient.query(params).promise();
    const now = moment().valueOf();

    //채팅을 DB에 저장한다.
    const item = {
        roomId: inputObject.roomId,
        timestamp: now,
        message: inputObject.text,
        userId: inputObject.userId,
        name: inputObject.name,
    };
    var params = {
        TableName: 'chatapp-chat-messages',
        Item: item
    };
    await docClient.put(params).promise();

    //이전에 불러온 방에 접속한 유저들 모두에게 채팅을 보낸다.
    const apigwManagementApi = new AWS.ApiGatewayManagementApi({
        apiVersion: '2018-11-29',
        endpoint: `${process.env.socket_api_gateway_id}.execute-api.ap-northeast-2.amazonaws.com/dev`
    });
    if (result.Items) {
        const postCalls = result.Items.map(async ({ connectionId }) => {
            const dt = { ConnectionId: connectionId, Data: JSON.stringify(item) };
            try {
                await apigwManagementApi.postToConnection(dt).promise();
            } catch (e) {
                console.log(e);
                //만약 이 접속은 끊긴 접속이라면, DB에서 삭제한다.
                if (e.statusCode === 410) {
                    console.log(`Found stale connection, deleting ${connectionId}`);
                    var params = {
                        TableName: 'chatapp-userlist',
                        Key: {
                            connectionId: connectionId
                        }
                    };
                    await docClient.delete(params).promise();
                }
            }
        });
        try {
            await Promise.all(postCalls);
        } catch (e) {
            return { statusCode: 500, body: e.stack };
        }
    }
    let response = {
        isBase64Encoded: true,
        statusCode: 200,
        headers: {
            "Content-Type": "application/json; charset=utf-8",
            "Access-Control-Expose-Headers": "*",
            "Access-Control-Allow-Origin": "*",
        },
        body: `{"users": ${JSON.stringify(result.Items)}}`
    };
    return response
};