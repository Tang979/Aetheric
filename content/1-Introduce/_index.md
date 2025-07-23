---
title: "Introduction"
date: "2025-07-08"
weight: 1
chapter: false
pre: " <b> 1. </b> "
---

The project titled **"Modernizing Mobile Game Backend with AWS Cloud Services"** focuses on building a **practical backend architecture** for a mobile game developed with **Unity 6**. The architecture follows a **serverless, cloud-native model**, eliminating the need for traditional servers and leveraging AWS services that **automatically scale and charge based on actual usage**.

## Advantages of the Solution

| Advantage                  | Description                                                                 |
|---------------------------|-----------------------------------------------------------------------------|
| Low Cost                  | No need to rent EC2 servers; all services are serverless, reducing operational costs during low traffic |
| Auto-scaling              | Lambda, DynamoDB, and API Gateway scale automatically based on demand without manual intervention |
| Native Unity Integration  | Uses UnityWebRequest for communication; no SDK needed, giving developers full control over request/response |
| Strong Security           | Authentication via JWT (Amazon Cognito), access controlled by IAM using the Least Privilege principle |
| Easy to Test & Debug      | API endpoints can be tested using Postman before integrating with Unity; centralized logs via CloudWatch |

## Limitations and Challenges

| Limitation / Challenge    | Description                                                                 |
|---------------------------|-----------------------------------------------------------------------------|
| No official SDK for Unity 6 | Requires manually signing requests using AWS Signature V4 or handling JWT authentication manually |
| Steep learning curve for IAM | Misconfigurations can easily break access or cause security risks if IAM permissions are not well understood |
| Difficult to emulate offline | Since most components depend on live AWS services, simulating the full backend offline is challenging |
| Potential cost growth at scale | While initially cost-effective, large-scale user bases require architecture reviews to optimize costs and performance |

## Who is this for?

- Game developers looking to learn and apply real-world cloud architecture tailored for mobile games.
- Student teams building a fast MVP without maintaining server infrastructure.
- AWS learners seeking an end-to-end, practical use case involving Lambda, Cognito, API Gateway, and DynamoDB.
