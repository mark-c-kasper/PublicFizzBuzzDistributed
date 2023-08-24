# Distributed FizzBuzz

This intent of this solution is to take a pretty simple programming problem and to expand it to a cloud-bases solution.

## Requirements

3 Lambda Functions. These are developed within this solution.
2 static SQS Queues.  For the purposes of this, the queues were named FizzBuzzFizzQueue and FizzBuzzBuzzQueue, respectively.
1 dynamically created SQS Queue.  This is given the name of a GUID as that follows the SQS naming convention.

## Lambda

The first lambda function has the following responsibilities:
- located in the LambdaASP.FizzBuzzApi project
- Acts as the entry point
- Contains a controller that will return the appropriate response to the given number
- Create a dynamic queue that it will later read from
- Send the number into the FizzBuzzFizzQueue
- Will poll the dynamic queue until it receives a message.
- Deletes the dynamic queue after the message has been received.

The second lambda function has these responsibilities:
- Located in the FizzProcessor project
- Reads from the FizzBuzzFizzQueue
- Will add "Fizz" to the message body if the value is divisible by 3.
- Writes to the FizzBuzzBuzzQueue

The third lambda function covers the following:
- Located in the BuzzProcessor project
- Reads from the FizzBuzBuzzQueue
- Will add "Buzz" to the message body if the value is divisible by 5.
- Writes to the queue that was created in the first lambda.

## Permissions
I will not cover a lot of the permissions outside of that I reused a custom IAM role for several different Lambda functions.
I manually added the full SQS role as I did not want to put forth the effort to isolate and add the sendmessage individual permission.
The other permissions the role had was to have readonly SQS access as two of the lambdas only interact with queues.

## Deployment commands:

Note:  In order for the `dotnet` commands to be successful, you will need to go back up to the FizzBuzzDistributed directory.

Deploy first lambda function:

```
cd "LambdaASP.FizzBuzzApi/src/LambdaASP.FizzBuzzApi"
dotnet lambda deploy-serverless
```

Deploy second lambda function:

```
cd "FizzProcessor/src/FizzProcessor"
dotnet lambda deploy-function
```

Deploy third lambda function:

```
cd "BuzzProcessor/src/BuzzProcessor"
dotnet lambda deploy-function
```

