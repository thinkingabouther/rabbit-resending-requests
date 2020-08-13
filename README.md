# rabbit-resending-requests

## Overview

Producer and Consumer that work with RabbitMQ. Producer send a message with URI of a resource. 
Consumer recevies the URI and procceses it according to the logic inside Requester. The default one makes GET request. If request fails (status-code is not 20*), the message resends after a given delay. 


## What is done
- [Producer](https://github.com/thinkingabouther/rabbit-resending-requests/blob/master/Producer/RabbitMessageProducer.cs) that sends a message given as an argument
- [Consumer](https://github.com/thinkingabouther/rabbit-resending-requests/blob/master/Consumer/RabbitMessageConsumer.cs) that processeses the message using [Requester](https://github.com/thinkingabouther/rabbit-resending-requests/blob/master/Consumer/Requesters/GetRequester.cs)

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
3. Run Producer
  - the default message will be sent:
  ```
  dotnet run --project Producer
  ```
  - You can also send custom message by passing it as an argument:
  ```
  dotnet run --project Consumer "https://yandex.com"
  ```
### Adding Requesters
Requester used to process the message is implemented using strategy pattern, so it will be easy for you add your own. 
To do so, implement [IRequester](https://github.com/thinkingabouther/rabbit-resending-requests/blob/master/Consumer/Requesters/IRequester.cs) interface for your class and pass it to Consumer constructor
