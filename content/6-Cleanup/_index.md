---
title: "Clean up resources"
date: "2025-07-08"
weight: 6
chapter: false
pre : " <b> 6. </b> "
---

### 1. Delete DynamoDB Table

Access the [AWS Management Console](https://ap-southeast-2.console.aws.amazon.com/console/home?region=ap-southeast-2) and follow these steps:

+ Navigate to the DynamoDB service and access the [DynamoDB Console](https://console.aws.amazon.com/dynamodbv2/home)
+ In the left navigation pane, select **Tables**
+ Find and select the table you want to delete from the list
+ Click the **Delete** button in the top right corner
+ In the confirmation dialog, enter `confirm` and select **Delete table**

### 2. Delete S3 Buckets

Access the [S3 Console](https://console.aws.amazon.com/s3) and perform the following steps:

+ Locate the bucket you want to delete
+ Select the bucket and click the **Empty** button
+ Enter `permanently delete` to confirm and select **Empty bucket**
+ Once the bucket is empty, select **Delete**
+ Confirm by entering the bucket name and select **Delete bucket**
