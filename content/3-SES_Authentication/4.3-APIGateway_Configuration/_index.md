---
title: "API Gateway Configuration"
date: "2025-07-08"
weight: 3
chapter: false
pre: " <b> 3.3. </b> "
---

1. Open [Amazon API Gateway](https://ap-southeast-2.console.aws.amazon.com/apigateway/main/apis?region=ap-southeast-2) and click the **Create an API** button to start the configuration process  
![Connect](/images/SES/4.3/4.3.1.png)

2. Next, configure the **Configure API** section:  
   + In the **API name** field, enter a name of your choice  
   + Under the **Integrations** section, choose **Lambda**.  
     In the **Lambda function** field, select the Lambda function you just created  
   + Once done, click **Next** to continue  
![Connect](/images/SES/4.3/4.3.2.png)

3. In the **Configure routes** step, under the **Method** dropdown, select **POST**.  
   After that, click **Next**  
![Connect](/images/SES/4.3/4.3.3.png)

4. In the final step, review all the configuration details to ensure everything is correct.  
   Once confirmed, click **Create** to finish setting up the API Gateway  
![Connect](/images/SES/4.3/4.3.4.png)

Now that the **API Gateway** has been configured, we will move on to integrating this service into the game.
