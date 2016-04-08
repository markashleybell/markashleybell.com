Title: Binding JavaScript object literals to ASP.NET MVC models with jQuery AJAX
Abstract: Can't get model binding working? Check this list of gotchas.
Published: 2016-04-08 07:48
Updated: 2016-04-08 07:48

Binding JavaScript object literals POSTed via AJAX to ASP.NET MVC models often raises issues. It's actually quite simple, but there are several gotchas:

## Create a specific model for your controller to bind values to

Model binding will bind single parameters:



But you can't POST lists of literals, so the following won't work:



To get around this, create a specific model for the consumer method, with the list you want to bind as a property:



Even if you're only binding to simple value types, it's a good idea to create a model to keep things consistent.

## Set the request content type

The jQuery AJAX function settings allow you to specify a content type for the request. Make sure this is set to `'application/json; charset=utf-8'`.



This tells the MVC model binder to treat the request body as JSON, rather than as a collection of request parameters.

## Convert your JavaScript object literal to a string before POSTing

In a normal jQuery AJAX POST request, an object literal is usually assigned to the data setting. The object's property names and values are then serialised into a POST request body.

But when we want to POST the object literal _itself_, we need to convert it into a string first. `JSON.stringify` will do this for us:











I hope this post helps other developers get past this particular stumbling block!