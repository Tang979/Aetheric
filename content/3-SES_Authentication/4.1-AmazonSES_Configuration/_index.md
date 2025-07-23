---
title: "Amazon SES Configuration"
date: "2025-07-08"
weight: 1
chapter: false
pre: " <b> 3.1. </b> "
---

1. Open [Amazon SES](https://ap-southeast-2.console.aws.amazon.com/ses/home?region=ap-southeast-2#/homepage)  
    + On the left sidebar under **Configuration**, select **Identities**  
![Connect](</images/SES/4.1.png>)

2. On the **Identities** page, click **Create identities**  
![Connect](</images/SES/4.2.png>)

3. On the **Create Identities** page:  
    + In the **Identity details** section, select **Email Address**  
    + Under **Email address**, enter your own email address. Finally, click **Create Identity**  
![Connect](</images/SES/4.3.png>)

4. Then, go to your email inbox and click the verification link sent by Amazon Web Services to verify the identity  
![Connect](</images/SES/4.4.png>)

5. After that, return to the **Identities** page to check whether the identity you just created has been **Verified**.  
If it's verified, it means you have successfully configured Amazon SES.  
![Connect](</images/SES/4.5.png>)

Next, we will configure **Lambda** to handle the email sending logic.
