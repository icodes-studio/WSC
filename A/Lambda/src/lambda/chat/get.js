var AWS = require("aws-sdk");
AWS.config.update({
    region: "ap-northeast-2"
});
exports.handler = async function (event, context) {
    var docClient = new AWS.DynamoDB.DocumentClient();
    var params = {
        TableName: 'chatapp-chat-messages',
        KeyConditionExpression: '#HashKey = :hkey',
        ExpressionAttributeNames: { '#HashKey': 'roomId' },
        ExpressionAttributeValues: {
            ':hkey': event.queryStringParameters.roomId
        }
    };
    try {
        const result = await docClient.query(params).promise();
        let response = {
            isBase64Encoded: true,
            statusCode: 200,
            headers: {
                "Content-Type": "application/json; charset=utf-8",
                "Access-Control-Expose-Headers": "*",
                "Access-Control-Allow-Origin": "*",
            },
            body: `{"messages": ${JSON.stringify(result.Items)}}`
        };
        return response
    }
    catch (e) {
        console.log(e)
        let response = {
            isBase64Encoded: true,
            statusCode: 500,
            headers: {
                "Content-Type": "application/json; charset=utf-8",
                "Access-Control-Allow-Origin": "*",
            },
            body: JSON.stringify("error")
        };
        return response;
    }
}
