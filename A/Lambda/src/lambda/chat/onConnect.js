const AWS = require('aws-sdk');
var moment = require('moment');
exports.handler = async (event) => {
    let inputObject = event.queryStringParameters;
    var docClient = new AWS.DynamoDB.DocumentClient();

    const item = {
        roomId: inputObject.roomId,
        connectionId: event.requestContext.connectionId,
        userId: inputObject.userId,
        timestamp: moment().valueOf()
    }
    try {
        var params = {
            TableName: 'chatapp-userlist',
            Item: item
        };
        await docClient.put(params).promise();
        let response = {
            isBase64Encoded: true,
            statusCode: 200,
            headers: {
                "Content-Type": "application/json; charset=utf-8",
                "Access-Control-Expose-Headers": "*",
                "Access-Control-Allow-Origin": "*",
            },
            body: "ok"
        };
        return response;
    } catch (e) {
        console.log(e);
        return "error";
    }

};