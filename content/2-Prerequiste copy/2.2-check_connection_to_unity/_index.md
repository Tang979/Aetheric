---
title: "Check Connection to Unity"
date: "2025-07-08"
weight: 2
chapter: false
pre: " <b> 2.2. </b> "
---

1. Go to the user you just created in the **User Pool**, and under the **Overview** section, copy the value `ap-southeast-2`.  
![Connect](/images/2.prerequisite/2.2/2.2.2.png)

2. In the **App client** section, copy the **Client ID**.  
![Connect](/images/2.prerequisite/2.2/2.2.1.png)

3. In your Unity project, locate the code section for the **Register** and **Confirm** pages. Create two variables: `clientId` and `region`. Assign the Client ID and Region values from your **User Pool**.  
![Connect](/images/2.prerequisite/2.2/2.2.3.png)

4. Enter your registration information and click **Register**.  
![Connect](/images/2.prerequisite/2.2/2.2.4.png)

5. After successful registration, you will receive a confirmation code via email. Copy the code.  
![Connect](/images/2.prerequisite/2.2/2.2.5.png)

6. Go back to the **Verify Email** section:  
    - Enter the **Confirmation Code** you received.  
    - Click the **Submit** button.  
![Connect](/images/2.prerequisite/2.2/2.2.6.png)

7. Go back to **Amazon Cognito**, navigate to **Users** under **User Management**.  
![Connect](/images/2.prerequisite/2.2/2.2.7.png)

8. Click on the newly created user, scroll down to the **User attributes** section, and you’ll see your user information.  
![Connect](/images/2.prerequisite/2.2/2.2.8.png)
