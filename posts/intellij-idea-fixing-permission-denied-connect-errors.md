Title: IntelliJ IDEA: Fixing "Permission denied: connect" Errors
Abstract: An issue with some older Cisco VPN client software means IntelliJ IDEA will throw connection exceptions when trying to perform online actions. Here's how to fix it.
Date: 2012-12-21 13:37

I'm making my first tentative steps with <a href="http://www.scala-lang.org/">Scala</a>, and according to many sources (including Mikio Braun's excellent <a href="http://blog.mikiobraun.de/2011/04/getting-started-in-scala.html">getting started article</a>), the IDE of choice is <a href="http://www.jetbrains.com/idea/">IntelliJ IDEA</a>. 

However, having downloaded and installed the Community Edition, I found I was unable to install the Scala plugin; when I clicked the `Browse Repositories` button under the Plugins dialog, IDEA threw up a `Permission denied: connect`
exception.

It turns out that this is due to an issue with Java 7 (which brings IPV6 support) and the Cisco AnyConnect VPN client I have installed (which seemingly doesn't support IPV6 sockets, see <a href="http://www.java.net/node/703177">here</a>).

The obvious workaround is to close the VPN client, but as I need this constantly it wasn't really practical. There is also an update to the client which may well fix the issue, but as we don't have access to download it due to Cisco's support system, the other way around this is as follows:

 1. Open the IntelliJ IDEA `bin` folder—in my case, `C:\Program Files (x86)\JetBrains\IntelliJ IDEA Community Edition 12.0.1\bin`
 2. Open the `idea.exe.vmoptions` and `idea64.exe.vmoptions` files
 3. Add the following on a new line at the end of each file:  
`-Djava.net.preferIPv4Stack=true`
 4. Save the files and restart IntelliJ IDEA

You should now find IDEA can connect to online endpoints successfully.
