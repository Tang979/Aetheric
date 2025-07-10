---
title: "Create DynamoDB Table and Configure Required Permissions"
date: "2025-07-08"
weight: 1
chapter: false
pre: " <b> 3.1. </b> "
---

<!-- ![SSMPublicinstance](/images/arc-02.png) -->

### Step 1: Create a DynamoDB Table

1. Go to the [DynamoDB Console](https://ap-southeast-2.console.aws.amazon.com/dynamodbv2/home).
   - Click **Create table**.
   - Enter your desired table name in the **Table name** field.
   - Specify a **Partition key** – this will serve as your table's primary key. You can also select the **data type** next to it.
   - Click **Create table** to finish.

<!-- ![Connect](/images/3.connect/001-connect.png) -->

### Step 2: Configure Cognito Identity Pool

1. Go to the [Cognito Console](https://ap-southeast-2.console.aws.amazon.com/cognito/).
   - Select **Identity pools**.
   - Click **Create identity pool**.
   - Choose **Authenticated access**, then select the login method(s) you've set up in **User pools**, then click **Next**.
   - Set the **IAM role name** and click **Next**.
   - Choose the **User Pool ID** and **App Client ID** you want to associate for authentication, then click **Next**.
   - Give your **Identity pool** a name, review your configuration, then click **Create identity pool**.

### Step 3: Create an IAM Policy to Grant Cognito Access to DynamoDB

1. Go to the [IAM Policies Console](https://us-east-1.console.aws.amazon.com/iam/home?region=ap-southeast-2#/policies).
   - Click **Create policy**.
   - Switch to the **JSON** tab and update it as follows:
     ```json
     {
       "Version": "2012-10-17",
       "Statement": [
         {
           "Effect": "Allow",
           "Action": [
             "dynamodb:GetItem",
             "dynamodb:PutItem",
             "dynamodb:UpdateItem"
           ],
           "Resource": "arn:aws:dynamodb:[Region]:*:table/[Table Name]"
         }
       ]
     }
     ```
   - Click **Next**, name the policy, and choose **Create policy**.

### Step 4: Attach Policy to Cognito Role and Edit Trust Policy

1. Go to the [IAM Console](https://us-east-1.console.aws.amazon.com/iam/home).
   - Click **Roles** from the left menu and find the IAM role created with your Identity Pool.
   - Click **Add permissions** → **Attach policies**.
   - Search and select the policy you just created → **Add permissions**.
   - Go to the **Trust relationships** tab → click **Edit trust policy**.
   - Replace the trust policy with:
     ```json
     {
       "Version": "2012-10-17",
       "Statement": [
         {
           "Effect": "Allow",
           "Principal": {
             "Federated": "cognito-identity.amazonaws.com"
           },
           "Action": "sts:AssumeRoleWithWebIdentity",
           "Condition": {
             "StringEquals": {
               "cognito-identity.amazonaws.com:aud": "[Identity pool ID]"
             },
             "ForAnyValue:StringLike": {
               "cognito-identity.amazonaws.com:amr": "authenticated"
             }
           }
         }
       ]
     }
     ```

