---
title: "Push Game Data from Unity to Amazon DynamoDB"
date: "2025-07-08"
weight: 3
chapter: false
pre: " <b> 3. </b> "
---

In this section, we will build the workflow that enables a **Unity 6 game client** to send game data (e.g., tower team, level progress, player state) to **Amazon DynamoDB**, using **API Gateway** and **AWS Lambda**, with authentication secured via **Amazon Cognito**.

Since Unity 6 no longer supports the official AWS SDK, all backend interactions are handled via **HTTP requests** using `UnityWebRequest`. Therefore, API-level security and access control must be configured strictly and precisely.

Before you can send and persist any data, you must first configure the necessary backend services. The **User Pool (Amazon Cognito)**, which manages user accounts and issues JWTs for API Gateway authorization, must be set up as described in the previous section.

### Content
3.1. [Create DynamoDB Table and Configure Required Permissions](3.1-public-instance/) \
<!-- 3.2. [Connect to EC2 Private Instance (Optional)](3.2-private-instance/) -->
