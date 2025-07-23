---
title: "Create User Pool"
date: "2025-07-08"
weight: 1
chapter: false
pre: " <b> 2.1. </b> "
---

1. Open the [Amazon Cognito console](https://ap-southeast-2.console.aws.amazon.com/cognito/v2/idp/user-pools?region=ap-southeast-2)  
    + Select **User pools** from the left-hand menu.  
    + Click **Create user pool**.  
![Connect](</images/2.prerequisite/user_pool/Screenshot 2025-07-09 100815.png>)

2. On the **Set up resources for your application** page  
    + Under **Define your application**, select **Mobile app**  
    + Under **Configure options**, select **Email**, **Username**  
![Connect](</images/2.prerequisite/user_pool/Screenshot 2025-07-09 101949.png>)  
    + Under **Required attributes for sign-up**, select **Email**  
    + Finally, click **Create User Directory**  
![Connect](</images/2.prerequisite/user_pool/Screenshot 2025-07-09 102420.png>)

3. Return to the [Amazon Cognito console](https://ap-southeast-2.console.aws.amazon.com/cognito/v2/idp/user-pools?region=ap-southeast-2), and click on the newly created user pool.  
![Connect](/images/2.prerequisite/user_pool/buoc4.png)

4. In the left menu, under **Application**, click **App client**.  
![Connect](/images/2.prerequisite/user_pool/buoc5.png)

5. Under **App clients and analytics**, click on the newly created app client name.  
![Connect](/images/2.prerequisite/user_pool/buoc6.png)  
    + In the **App client** section, click **Edit**.

6. In the **Edit app client information** section, enable the following options:  
    + **Choice-based sign-in**: `ALLOW_USER_AUTH`  
    + **Sign in with username and password**: `ALLOW_USER_PASSWORD_AUTH`  
    + **Get new user tokens from existing authenticated sessions**: `ALLOW_REFRESH_TOKEN_AUTH`  
Then scroll to the bottom and click **Save Changes**.  
![Connect](/images/2.prerequisite/user_pool/buoc7.png)

That completes the process of creating a **User Pool**.