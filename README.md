# rabbit-resending-requests

## Overview

Producer and Consumer that work with RabbitMQ. Producer send a message with URI of a resource. 
Consumer receives the URI and processes it according to the logic inside Requester. The default one makes GET request. If request fails (status-code is not 20*), the message resends after a given delay using dead-letter exchange (DLX) via Republisher. 


## What is done
- [Producer](https://github.com/thinkingabouther/rabbit-resending-requests/blob/master/Producer/RabbitMessageProducer.cs) that sends a message given as an argument
- [Consumer](https://github.com/thinkingabouther/rabbit-resending-requests/blob/master/Consumer/RabbitMessageConsumer.cs) that processes the message using [Requester](https://github.com/thinkingabouther/rabbit-resending-requests/blob/master/Consumer/Requesters/GetRequester.cs)
- [Rebulisher](https://github.com/thinkingabouther/rabbit-resending-requests/blob/master/Consumer/FailurePostProcessors/RabbitMessageRePublisher.cs) that resends the message back to a consumer after a given delay

## What might be done in the future
- Custom growing TTL for message that was rejected

## Requirements
- .NET Core 3 or newer
- RabbitMQ 3 or newer

## Usage
### Testing the app
1. Clone the project and move to its directory
2. Run Consumer:
```
dotnet run --project Consumer
```
3. Open another terminal instance and run Producer
  - the default message will be sent:
  ```
  dotnet run --project Producer
  ```
  - You can also send custom message by passing it as an argument:
  ```
  dotnet run --project Producer "https://yandex.com"
  ```
### Adding Requesters
Requester used to process the message is implemented using strategy pattern, so it will be easy for you add your own. 
To do so, implement [IRequester](https://github.com/thinkingabouther/rabbit-resending-requests/blob/master/Consumer/Requesters/IRequester.cs) interface for your class and pass it to Consumer constructor

The same goes with post-processing the message that was acknowledged negatively by the Requester. In that case, 
Requester uses an instance implementing [IFailurePostProcessor](https://github.com/thinkingabouther/rabbit-resending-requests/blob/master/Consumer/FailurePostProcessors/IFailurePostProcessor.cs)
to do something with that message (i.e. log to DB, republish back to rabbit). You can add it on your own using the same pattern as with IRequester
