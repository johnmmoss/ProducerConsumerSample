# C# Producer/Consumer

Some sample code that explores the producer/consumer pattern from StackOverflow [C# Producer and Consumer](http://stackoverflow.com/questions/1656404/c-sharp-producer-consumer).

This post references:

* [Threading in C# (Joe Albahari)](http://www.albahari.com/threading/)
* [Multi-threading in .NET: Introduction and suggestions] (http://www.yoda.arachsys.com/csharp/threads/)
* [BlockingCollection Overview (MSDN)](https://msdn.microsoft.com/en-us/library/dd997371.aspx)

## ProducerConsumerTest
A simple producer/consumer with single producer and single consumer based on the code provided on the StackOverflow post.

## ProducerMultipleConsumerTest
A multiple consumer version of the previous example based on code provided in the StackOverflow post.

## WebImageDownloader
An application used to do some real life work based on the previous test apps.
Scrapes a list of image Urls from a specified Url into a queue which acts as a producer.
A consumer then downloads the images to the currently executing directory.
Bit rough around the edges but does the job.
