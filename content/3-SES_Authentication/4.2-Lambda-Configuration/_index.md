---
title: "Lambda Configuration"
date: "2025-07-08"
weight: 2
chapter: false
pre: " <b> 3.2. </b> "
---

1. Open [Lambda](https://ap-southeast-2.console.aws.amazon.com/lambda/home?region=ap-southeast-2#/discover).  
   In the **Dashboard**, click **Create function**  
![Connect](</images/SES/4.2/4.2.1.png>)

2. On the **Create function** page, follow these steps:  
   + In **Function name**, enter your desired function name  
   + In the **Runtime** section, choose the programming language for your email handling logic.  
     In this case, select **Python 3.12**  
   + Under **Permissions**, select **Create a new role with basic Lambda permissions**  
   + Scroll to the bottom and click **Create function**  
![Connect](</images/SES/4.2/4.2.2.png>)

3. After the function is created, scroll down to the **Code** section and paste in the following code.  
   Then click the **Deploy** button on the left side to save it:

```python
import boto3
import json

ses = boto3.client('ses', region_name='ap-southeast-2')

def lambda_handler(event, context):
    body = json.loads(event['body'])

    sender = "minhallk.vk@gmail.com"  # Verified sender email
    to_email = "minhallk.vk@gmail.com"  # Receiver email for feedback
    subject = body.get("subject", "User Feedback")
    message = body.get("message", "No message provided")
    user_email = body.get("user_email", "Not provided")

    # Send email using SES
    response = ses.send_email(
        Source=sender,
        Destination={
            'ToAddresses': [to_email]
        },
        Message={
            'Subject': {
                'Data': subject
            },
            'Body': {
                'Text': {
                    'Data': f"From: {user_email}\n\nMessage:\n{message}"
                }
            }
        }
    )

    return {
        'statusCode': 200,
        'body': json.dumps({'message': 'Feedback sent successfully!'})
    }
