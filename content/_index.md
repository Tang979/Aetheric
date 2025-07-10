---
title: "Modernizing Mobile Game Backend with AWS Cloud Services"
date: "2025-07-08"
weight: 1
chapter: false
---

# Modernizing Mobile Game Backend with AWS Cloud Services

### Overview

As part of this workshop, our team presents the process of integrating core AWS services into a mobile tower defense game developed with Unity 6. Since **Unity 6 no longer officially supports AWS SDKs**, we designed the backend entirely with a **serverless-first approach**, using **REST APIs** authenticated via **AWS Signature Version 4** or **JWT tokens from Amazon Cognito**.

Each component is configured and integrated independently to ensure both security and scalability, while maintaining modularity and ease of testing. The overall implementation follows practical, production-ready principles while adhering to AWS's Well-Architected Framework.

<!-- ![ConnectPrivate](/images/arc-log.png)  -->

### Table of Contents

1. [Introduction](1-introduce/)
2. [Cognito User Authentication Setup](2-Prerequiste/)
3. [Push Game Data from Unity to Amazon DynamoDB](3-Accessibilitytoinstance/)
