---
title: "Introduction to AWS Services Used"
date: "2025-07-08"
weight: 1
chapter: false
pre: " <b> 1. </b> "
---

### Amazon Cognito Service

**Amazon Cognito** is an AWS service that helps manage and authenticate users securely **without the need to build your own registration, login, or password management system**. Cognito supports login via email, phone number, social logins (Google, Facebook, Apple), or enterprise accounts using SAML. It issues OAuth2-compliant security tokens that can be used to access backend services.

Cognito allows centralized user management via IAM, supports multi-factor authentication (MFA), fine-grained role-based access control, and provides virtually unlimited scalability. It also logs sign-in activities and errors to enhance traceability and security.

Key benefits of using Amazon Cognito include:

- No need to implement your own login, password reset, or email verification systems.
- Supports multiple login methods: email, social accounts, and enterprise identity providers.
- Centralized user management with IAM integration and clear role separation.
- Easy integration with API Gateway, Lambda, DynamoDB, S3, and more.
- Logs user activity to increase system visibility and security.
- Automatically scales with no server infrastructure management.

With these advantages, Cognito eliminates the need for bastion hosts or custom session management, saving time and infrastructure cost when building user authentication for your backend.

---

### Amazon SES (Simple Email Service)

**Amazon SES** is AWS’s scalable and secure email-sending service that allows you to **automatically send emails from your app or game without hosting your own mail server**. It supports use cases such as email confirmation, password reset, and user feedback — all with high deliverability and robust security.

You can configure SES to send email from a custom sender address (e.g., support@yourgame.com) and integrate authentication mechanisms like SPF and DKIM to prevent spam flags. It also works seamlessly with Lambda or API Gateway, so clients like Unity can trigger emails securely without exposing sensitive credentials.

Key benefits of using Amazon SES include:

- Send automated email confirmations, password resets, or feedback responses.
- No need to set up or maintain your own mail infrastructure.
- Easily integrates with services like Lambda, API Gateway, and S3.
- Supports domain authentication (SPF, DKIM) for improved email trust.
- Monitor delivery rates, bounce rates, and spam reports.
- Low cost, with thousands of free emails per month within AWS Free Tier.

With these benefits, Amazon SES is a secure, reliable, and cost-effective solution for implementing in-game email delivery features without worrying about backend complexity.

---

### Amazon DynamoDB Service

**Amazon DynamoDB** is AWS’s fully managed NoSQL database service that allows you to **store and retrieve data quickly and flexibly without managing any server infrastructure**. With DynamoDB, you can easily store data like player accounts, scores, game progress, inventory, or tower upgrade information.

DynamoDB operates as a key-value and document database that delivers millisecond latency even at high scale — supporting millions of concurrent players. It automatically scales, offers multi-region replication, and integrates with IAM for fine-grained access control.

Key benefits of using Amazon DynamoDB include:

- Stable storage and fast retrieval of player data with low latency.
- No need to install, maintain, or manually scale the database.
- Seamless integration with Lambda, API Gateway, and Cognito.
- Supports conditional queries and secondary indexes for fast lookups.
- Provides backup and restore features, as well as detailed access control.
- Ideal for backend infrastructure of games, mobile apps, and web apps with cost efficiency.

With these strengths, DynamoDB is a solid choice for storing large-scale game data with high performance and minimal operational cost.