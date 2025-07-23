---
title: "Connect to Unity"
date: "2025-07-08"
weight: 4
chapter: false
pre: " <b> 3.4. </b> "
---

1. Reopen **API Gateway** and access the API you just created.  
   + On the right side, under the **Deploy** section, select **Stages**  
   + In the **Stages** panel, click on **$default** to open the **Stages Detail** view  
   + In the **Stages Detail**, copy the provided **URL**  
![Connect](/images/SES/4.4/4.4.1.png)

2. Reopen your **Lambda** function, scroll down to the code, and locate the **sender** and **to_email** variables.  
   Replace them with your own email address  
![Connect](/images/SES/4.4/4.4.3.png)

3. In your game, create a simple feedback form for testing.  
   Log in using one of your other accounts  
![Connect](/images/SES/4.4/4.4.2.png)

4. Create a script to handle feedback submissions  
![Connect](/images/SES/4.4/4.4.5.png)  
   Paste the **URL** you copied from API Gateway into the script

5. Once everything is set up, run a test  
![Connect](/images/SES/4.4/4.4.4.png)  
   After sending feedback, check your main email inbox to see if the feedback was received  
![Connect](/images/SES/4.4/4.4.6.png)  

And that’s it — you have successfully used the **Amazon SES** service to send feedback from Unity to your email inbox!
