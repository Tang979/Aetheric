---
title: "Account Authentication with Amazon Cognito"
date:  "2025-07-08" 
weight: 2 
chapter: false
pre: " <b> 2. </b> "
---

{{% notice info %}}
You need to prepare one **Linux instance** in the **public subnet** and one **Windows instance** in the **private subnet** to complete this lab.
{{% /notice %}}

To learn how to create a user pool, you can refer to the lab:  
  - [Authentication with Amazon Cognito](https://000081.awsstudygroup.com/vi/)

To use **AWS Systems Manager** to manage the Windows instance specifically — and instances in general — we need to grant proper permissions for our instances to work with **Systems Manager**.  
In this preparation step, we will also create an **IAM Role** to allow instances to interact with Systems Manager.

### Contents
  - [Create User Pool](2.1-createuserpool/)