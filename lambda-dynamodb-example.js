// Import AWS SDK
const AWS = require('aws-sdk');

// Khởi tạo DynamoDB client
const dynamodb = new AWS.DynamoDB.DocumentClient();

exports.handler = async (event) => {
    try {
        // Lấy dữ liệu từ event
        const data = JSON.parse(event.body || '{}');
        const { userId, score, level } = data;
        
        if (!userId) {
            return {
                statusCode: 400,
                body: JSON.stringify({ message: 'userId là bắt buộc' })
            };
        }
        
        // Tham số để lưu vào DynamoDB
        const params = {
            TableName: 'GameScores',
            Item: {
                userId: userId,
                timestamp: new Date().toISOString(),
                score: score || 0,
                level: level || 1,
            }
        };
        
        // Lưu dữ liệu vào DynamoDB
        await dynamodb.put(params).promise();
        
        // Trả về kết quả thành công
        return {
            statusCode: 200,
            body: JSON.stringify({
                message: 'Dữ liệu đã được lưu thành công',
                data: params.Item
            })
        };
    } catch (error) {
        // Xử lý lỗi
        console.error('Lỗi:', error);
        return {
            statusCode: 500,
            body: JSON.stringify({
                message: 'Đã xảy ra lỗi khi xử lý yêu cầu',
                error: error.message
            })
        };
    }
};