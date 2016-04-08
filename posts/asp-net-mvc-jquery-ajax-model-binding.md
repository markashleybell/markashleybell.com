Title: Binding JavaScript object literals to ASP.NET MVC models with jQuery AJAX
Abstract: Can't get model binding working when POSTing object literals via AJAX? Check these gotchas.
Published: 2016-04-08 07:48
Updated: 2016-04-08 07:48

Binding JavaScript object literals POSTed via AJAX to ASP.NET MVC models often raises issues. It's actually very simple, but there are a couple of gotchas which I always forget about:

## Make sure you set the correct content type for the request

The jQuery AJAX function settings allow you to specify a content type for the request. Make sure this is set to `'application/json; charset=utf-8'`.

This tells the MVC model binder to treat the request body as JSON, rather than as a collection of request parameters.

## Convert your JavaScript object literal to a string before POSTing

In a normal jQuery AJAX POST request, an object literal is usually assigned to the `data` setting. The object's property names and values are then serialised into a POST request body.

But when we want to POST the object literal _itself_, we need to convert it into a string representation first. `JSON.stringify` will do this for us.

Here's an incredibly simple example:

## C# Model

    ::csharp
    public class UserModel
    {
        public int ID { get; set; }
        public string Email { get; set; }
    }

## AJAX request

    ::javascript
    $.ajax({
        url: 'Home/YourPostAction',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ id: 100, email: 'test1@test.com' }),
        type: 'POST'
    });

## Controller Action Method

    ::csharp
    [HttpPost]
    public ActionResult PostAction(UserModel model)
    {
        // model.ID = 100
        // model.Email = "test1@test.com"
        return Json(model);
    }

I hope this post helps others get past these particular stumbling blocks!