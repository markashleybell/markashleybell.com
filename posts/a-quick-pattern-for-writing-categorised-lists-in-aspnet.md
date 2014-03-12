Title: A quick pattern for writing categorised lists in ASP.NET
Abstract: A simple method of writing out a set of categorised lists in ASP.NET/C#.
Published: 2010-02-12 09:46
Updated: 2010-02-12 09:46

This one's more for my own reference than anything else. I often need to write out a set of HTML lists, separated by headings; the code here is actually taken from the links page on my design blog, which you can take a look at as an example of what I mean (table and field names have been changed for security reasons).

Well, here's one simple way to do it. The code below allows everything to be stored in one database table, including the categories: we simply loop through and write out a heading when the category name changes.

    :::csharp
    string sql = "select title, url, category from links order by category, title";
        
    using (SqlDataReader links = new SqlCommand(sql, conn).ExecuteReader())
    {
        if(links.HasRows)
        {
            // Store the first category and write out the first heading
            string category = links["category"].ToString();
            Response.Write("<h2>" + category + "</h2><ul>");
        
            while (links.Read())
            {
                // If this link's category is different to the stored one
                if(links["category"].ToString() != category)
                {
                    // We've changed category, so store the new category 
                    // and write out a category heading 
                    category = links["category"].ToString();
                    Response.Write("</ul><h2>" + category + "</h2><ul>");
                }
                
                Response.Write("<li>Write out your links here</li>");
            }
            
            Response.Write("</ul>");
        }
    }

The table structure isn't normalised, but it does have its advantages: SQL queries are very simple and you can create new categories on the fly. Plus, for something this tiny, the performance and data integrity issues aren't even worth worrying about.