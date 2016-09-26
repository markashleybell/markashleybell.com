Title: Helping The Next Developer: Readable C# Code
Abstract: A few tips for writing C# code which explains itself clearly. 
Published: 2016-09-26 17:00
Updated: 2016-09-26 17:00

Today I'd like to share a few tips for writing C# code which explains itself clearly. This in turn will reduce the cognitive load for anyone maintaining your code.

These are not hard rules—I've definitely broken them on more than a few occasions—but it's worth keeping them in mind.

## White space and indentation

Indentation and white space help readers to parse related code blocks, so keep them consistent. Mismatched indentation can make your code much, much more difficult to understand. Following on from this...

## Reduce line length

Shorter lines are more readable.

See what I did there? You can indent fluent interface calls (e.g. LINQ extension methods) to make them a lot easier to scan:

    :::csharp
    var resultList = items.SelectMany(x => x.IntArray)
                          .Select(x =&gt; x[1])
                          .Distinct()
                          .ToList();

Another common culprit when it comes to lengthy lines is the ternary expression. I find it helpful to break longer expressions onto two lines:

    :::csharp
    var intVar = (myVal == 1) ? Convert.ToInt32(ConfigurationManager.AppSettings["Flag"])
                              : new DynamicConfiguration(myVal).GetValue();

In the case above, it might be even clearer to rewrite the ternary expression as an if/else statement.

## Use implicitly typed variables where possible

Use the `var` keyword to reduce noise when initialising variables. There's no need for this:

    :::csharp
    RatherLengthyClassName myObject = new RatherLengthyClassName();

When you can do this, with no loss of clarity:

    :::csharp
    var myObject = new RatherLengthyClassName();

The exception to this rule is when it isn't at all clear what the type will be from the assignment:

    :::csharp
    Animal dog = GetThing(true);

This contrived example contains a deliberately terrible method name, but we've all seen similar in real codebases...

## Named method parameters

Doubtless you will have encountered lines like the following:

    :::csharp
    var myObject1 = new ComplicatedThing(true, 3, true, false, true);

    myObject2.PerformAction(Action.Update, 12, "Joe Bloggs", true);

But what do all those parameter values represent? Using named method parameters can make things much clearer for maintainers:

    :::csharp
    var myObject1 = new ComplicatedThing(
        loadChildren: true,
        maxDepth: 3,
        loadImages: true,
        allowUpdates: false,
        lockDeletion: true
    );

    myObject2.PerformAction(
        action: Action.Update,
        id: 12,
        name: "Joe Bloggs",
        subscribe: true
    );

You don't *have* to pass in names for all the parameters, so you can omit them where the meaning is obvious:

    :::csharp
    myObject3.AddNewItem(itemData, redirect: true);

## Commenting: remember “WHY, not WHAT”

As a general rule of thumb, don't use comments to describe *what* a piece of code does. If it's not clear from reading the code itself, you should try to simplify it until the meaning *is* obvious.

Add comments when you want to explain *why* you did something. That way, maintainers won't have to spend time trying to figure out what you were thinking when you wrote it.

## Remove old code

*Don't* just comment out old, unused code and leave it there “just in case”. Having to scan past large blocks of unnecessary commented-out code just increases cognitive load.

If code is no longer used, *delete it and make a commit explaining why you did so*. You can always retrieve it from your version control system if you discover you need it again later.

<br><br>
I hope you find these tips useful! As always, your comments are welcome.
