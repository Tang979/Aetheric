---
title : "Modernizing Mobile Game Backend with AWS Cloud Services"
date :  "2025-07-08" 
weight : 1 
chapter : false
---
# Modernizing Mobile Game Backend with AWS Cloud Services

## Overview

In this workshop, our team presents the process of integrating core AWS services into a tower defense mobile game project developed on the Unity 6 platform. Since **Unity 6 has discontinued official support for the AWS SDK**, we've built the entire backend architecture following a **serverless** approach and implemented communication with AWS services through **REST APIs** signed with **AWS Signature Version 4** or authenticated with **JWT from Amazon Cognito**.

Configuration and integration are performed in separate functional blocks, ensuring the system is secure, scalable, and easy to test independently. The entire deployment process is designed with a practical approach – prioritizing efficiency and simplicity while still adhering to AWS Well-Architected Framework principles.

![conectprivate](images/mobile_game_serverless_data_flow.png)

### Contents

 1. [Introduction](1-introduce/)
 2. [Cognito Account Authentication](2-Prerequiste/)
 3. [Using Amazon SES to Send Emails from Users to Server](3-SES_Authentication/)
 4. [Pushing Data from Unity to Amazon DynamoDB](4-Accessibilitytoinstance/)
 5. [Using S3 to Load Dynamic Assets for Unity Games](5-S3WithUnity/)
 6. [Clean up Resources](6-Cleanup/)
