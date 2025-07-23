---
title : "Create S3 Bucket for Game Assets"
date :  "2025-07-08" 
weight : 1 
chapter : false
pre : " <b> 5.1. </b> "
---

### Access Amazon S3

1. Access the [S3](https://ap-southeast-2.console.aws.amazon.com/s3/home?region=ap-southeast-2#) page
2. Click on the **Create bucket** button

![S3Console](/images/4.s3/s3-console.png)

### Configure the bucket

1. **Basic information**:
   - **Bucket name**: Enter your bucket name (example: `mygame-assets`)

   ![S3Name](/images/4.s3/s3-name.png)

2. **Object Ownership**: Keep the default "ACLs disabled"

3. **Block Public Access settings**: By default, S3 will block all public access. To allow assets to be loaded from your game, uncheck "Block all public access"

   ![S3Public](/images/4.s3/s3-public.png)

4. Click **Create bucket**

### Configure CORS (Cross-Origin Resource Sharing)

CORS allows your Unity game to load assets from the S3 bucket:

1. Select the bucket you just created
2. Select the **Permissions** tab
3. Scroll down to the **Cross-origin resource sharing (CORS)** section
4. Click **Edit**
5. Enter the following CORS configuration:

    ```json
    [
        {
            "AllowedHeaders": [
                "*"
            ],
            "AllowedMethods": [
                "GET",
                "HEAD"
            ],
            "AllowedOrigins": [
                "*"
            ],
            "ExposeHeaders": []
        }
    ]
    ```

6. Click **Save changes**

![S3CORS](/images/4.s3/s3-cors.png)

### Create a Bucket Policy

To allow public access to assets:

1. Still in the **Permissions** tab, scroll to the **Bucket policy** section
2. Click **Edit**
3. Enter the following policy (replace `mygame-assets` with your bucket name):

    ```json
    {
        "Version": "2012-10-17",
        "Statement": [
            {
            "Sid": "PublicReadForGetBucketObjects",
            "Effect": "Allow",
            "Principal": "*",
            "Action": "s3:GetObject",
            "Resource": "arn:aws:s3:::mygame-assets/*"
            }
        ]
    }
    ```

4. Click **Save changes**

![S3Policy](/images/4.s3/s3-policy.png)

### Upload assets to S3

1. Go back to the **Objects** tab
2. Click **Create folder** to create a folder structure (example: `textures`, `models`, `audio`, `bundles`)
3. To upload assets:
   - Select the destination folder
   - Click **Upload**
   - Select files from your computer
   - Click **Upload**

### Get asset URLs

After uploading, you can get the URL of an asset to use in Unity:

1. Select the uploaded asset
2. Navigate to the file you uploaded to see the **Object URL**
3. The URL will look like: `https://mygame-assets.s3.amazonaws.com/textures/character.png`

### Security notes

The above configuration allows anyone to access your assets. In a production environment, you should:

1. Use CloudFront with OAI (Origin Access Identity) to restrict direct access to S3
2. Use Signed URLs or Cookies to control access
3. Consider using AWS WAF to protect against attacks
