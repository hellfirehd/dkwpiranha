using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Blocks;
using Piranha.Models;
using RazorWeb.Models;

namespace RazorWeb.Controllers;

/// <summary>
/// This controller is only used when the project is first started
/// and no pages has been added to the database. Feel free to remove it.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public class SetupController : Controller
{
    private readonly IApi _api;

    public SetupController(IApi api)
    {
        _api = api;
    }

    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("/seed")]
    public async Task<IActionResult> Seed()
    {
        // Get the default site
        var site = await _api.Sites.GetDefaultAsync();

        // Create a simple home page with syntax highlighting examples
        var startPage = await StandardPage.CreateAsync(_api);
        startPage.Id = Guid.NewGuid();
        startPage.SiteId = site.Id;
        startPage.Title = "Syntax Highlighting Test";
        startPage.NavigationTitle = "Home";
        startPage.MetaTitle = "Syntax Highlighting with Prism.js";
        startPage.MetaKeywords = "Syntax, Highlighting, Code, Prism";
        startPage.MetaDescription = "Testing syntax highlighting for various programming languages.";
        startPage.Excerpt = "A collection of code examples to test syntax highlighting functionality.";
        startPage.Published = DateTime.Now;

        // Introduction
        startPage.Blocks.Add(new HtmlBlock
        {
            Body =
                "<h2>Syntax Highlighting Examples</h2>" +
                "<p class=\"lead\">This page demonstrates syntax highlighting for multiple programming languages using our custom Markdig renderer and Prism.js integration.</p>"
        });

        // C# Example
        startPage.Blocks.Add(new MarkdownBlock
        {
            Body =
                "### C# Example\n\n" +
                "```csharp\n" +
                "public class HelloWorld\n" +
                "{\n" +
                "    private readonly ILogger<HelloWorld> _logger;\n" +
                "\n" +
                "    public HelloWorld(ILogger<HelloWorld> logger)\n" +
                "    {\n" +
                "        _logger = logger ?? throw new ArgumentNullException(nameof(logger));\n" +
                "    }\n" +
                "\n" +
                "    public async Task<String> SayHelloAsync(String name)\n" +
                "    {\n" +
                "        _logger.LogInformation(\"Saying hello to {Name}\", name);\n" +
                "        await Task.Delay(100);\n" +
                "        return $\"Hello, {name}!\";\n" +
                "    }\n" +
                "}\n" +
                "```"
        });

        // JavaScript/TypeScript Examples
        startPage.Blocks.Add(new MarkdownBlock
        {
            Body =
                "### JavaScript Example\n\n" +
                "```javascript\n" +
                "async function fetchUserData(userId) {\n" +
                "    try {\n" +
                "        const response = await fetch(`/api/users/${userId}`);\n" +
                "        if (!response.ok) {\n" +
                "            throw new Error(`HTTP error! status: ${response.status}`);\n" +
                "        }\n" +
                "        const data = await response.json();\n" +
                "        return data;\n" +
                "    } catch (error) {\n" +
                "        console.error('Failed to fetch user data:', error);\n" +
                "        return null;\n" +
                "    }\n" +
                "}\n" +
                "```\n\n" +
                "### TypeScript Example\n\n" +
                "```typescript\n" +
                "interface User {\n" +
                "    id: number;\n" +
                "    name: string;\n" +
                "    email: string;\n" +
                "}\n" +
                "\n" +
                "class UserService {\n" +
                "    private users: Map<number, User> = new Map();\n" +
                "\n" +
                "    addUser(user: User): void {\n" +
                "        this.users.set(user.id, user);\n" +
                "    }\n" +
                "\n" +
                "    getUser(id: number): User | undefined {\n" +
                "        return this.users.get(id);\n" +
                "    }\n" +
                "}\n" +
                "```"
        });

        // JSON Example
        startPage.Blocks.Add(new MarkdownBlock
        {
            Body =
                "### JSON Configuration Example\n\n" +
                "```json\n" +
                "{\n" +
                "  \"name\": \"piranha-cms\",\n" +
                "  \"version\": \"10.0.0\",\n" +
                "  \"description\": \"Open source CMS for ASP.NET Core\",\n" +
                "  \"dependencies\": {\n" +
                "    \"Markdig\": \"0.40.0\",\n" +
                "    \"Microsoft.AspNetCore.App\": \"9.0.0\"\n" +
                "  },\n" +
                "  \"features\": [\n" +
                "    \"Syntax Highlighting\",\n" +
                "    \"Markdown Support\",\n" +
                "    \"Extensible Architecture\"\n" +
                "  ],\n" +
                "  \"enabled\": true,\n" +
                "  \"maxConnections\": 100\n" +
                "}\n" +
                "```"
        });

        // PowerShell Examples
        startPage.Blocks.Add(new MarkdownBlock
        {
            Body =
                "### PowerShell Example\n\n" +
                "```powershell\n" +
                "# Deploy the application\n" +
                "function Deploy-Application {\n" +
                "    param(\n" +
                "        [Parameter(Mandatory=$true)]\n" +
                "        [string]$Environment,\n" +
                "        [string]$Configuration = \"Release\"\n" +
                "    )\n" +
                "\n" +
                "    Write-Host \"Deploying to $Environment...\" -ForegroundColor Green\n" +
                "    \n" +
                "    dotnet publish -c $Configuration\n" +
                "    if ($LASTEXITCODE -ne 0) {\n" +
                "        Write-Error \"Build failed!\"\n" +
                "        return\n" +
                "    }\n" +
                "\n" +
                "    Write-Host \"Deployment complete!\" -ForegroundColor Green\n" +
                "}\n" +
                "\n" +
                "Deploy-Application -Environment \"Production\"\n" +
                "```\n\n" +
                "Testing `ps` alias:\n\n" +
                "```ps\n" +
                "Get-Process | Where-Object { $_.CPU -gt 100 } | Select-Object Name, CPU\n" +
                "```"
        });

        // Bash Example
        startPage.Blocks.Add(new MarkdownBlock
        {
            Body =
                "### Bash Script Example\n\n" +
                "```bash\n" +
                "#!/bin/bash\n" +
                "\n" +
                "# Build and run Docker container\n" +
                "CONTAINER_NAME=\"piranha-cms\"\n" +
                "IMAGE_TAG=\"latest\"\n" +
                "\n" +
                "echo \"Building Docker image...\"\n" +
                "docker build -t $CONTAINER_NAME:$IMAGE_TAG .\n" +
                "\n" +
                "if [ $? -eq 0 ]; then\n" +
                "    echo \"Build successful! Starting container...\"\n" +
                "    docker run -d -p 8080:80 --name $CONTAINER_NAME $CONTAINER_NAME:$IMAGE_TAG\n" +
                "    echo \"Container started at http://localhost:8080\"\n" +
                "else\n" +
                "    echo \"Build failed!\" >&2\n" +
                "    exit 1\n" +
                "fi\n" +
                "```"
        });

        // Python Example
        startPage.Blocks.Add(new MarkdownBlock
        {
            Body =
                "### Python Example\n\n" +
                "```python\n" +
                "from typing import List, Optional\n" +
                "import asyncio\n" +
                "\n" +
                "class DataProcessor:\n" +
                "    def __init__(self, data: List[int]):\n" +
                "        self.data = data\n" +
                "    \n" +
                "    async def process_async(self) -> Optional[int]:\n" +
                "        \"\"\"Process data asynchronously and return the sum.\"\"\"\n" +
                "        await asyncio.sleep(0.1)\n" +
                "        return sum(self.data) if self.data else None\n" +
                "\n" +
                "# Usage\n" +
                "processor = DataProcessor([1, 2, 3, 4, 5])\n" +
                "result = asyncio.run(processor.process_async())\n" +
                "print(f\"Result: {result}\")\n" +
                "```"
        });

        // HTML/CSS Examples
        startPage.Blocks.Add(new MarkdownBlock
        {
            Body =
                "### HTML Example\n\n" +
                "```html\n" +
                "<!DOCTYPE html>\n" +
                "<html lang=\"en\">\n" +
                "<head>\n" +
                "    <meta charset=\"UTF-8\">\n" +
                "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\n" +
                "    <title>Piranha CMS</title>\n" +
                "    <link rel=\"stylesheet\" href=\"/assets/css/style.css\">\n" +
                "</head>\n" +
                "<body>\n" +
                "    <header>\n" +
                "        <h1>Welcome to Piranha CMS</h1>\n" +
                "    </header>\n" +
                "    <main>\n" +
                "        <p>Content goes here...</p>\n" +
                "    </main>\n" +
                "</body>\n" +
                "</html>\n" +
                "```\n\n" +
                "### CSS Example\n\n" +
                "```css\n" +
                ":root {\n" +
                "    --primary-color: #007bff;\n" +
                "    --text-color: #333;\n" +
                "    --spacing: 1rem;\n" +
                "}\n" +
                "\n" +
                "body {\n" +
                "    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;\n" +
                "    color: var(--text-color);\n" +
                "    line-height: 1.6;\n" +
                "}\n" +
                "\n" +
                ".container {\n" +
                "    max-width: 1200px;\n" +
                "    margin: 0 auto;\n" +
                "    padding: var(--spacing);\n" +
                "}\n" +
                "```"
        });

        // SQL Example
        startPage.Blocks.Add(new MarkdownBlock
        {
            Body =
                "### SQL Example\n\n" +
                "```sql\n" +
                "-- Create a users table\n" +
                "CREATE TABLE Users (\n" +
                "    Id INT PRIMARY KEY IDENTITY(1,1),\n" +
                "    UserName NVARCHAR(100) NOT NULL UNIQUE,\n" +
                "    Email NVARCHAR(255) NOT NULL,\n" +
                "    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),\n" +
                "    IsActive BIT DEFAULT 1\n" +
                ");\n" +
                "\n" +
                "-- Insert sample data\n" +
                "INSERT INTO Users (UserName, Email)\n" +
                "VALUES \n" +
                "    ('alice', 'alice@example.com'),\n" +
                "    ('bob', 'bob@example.com');\n" +
                "\n" +
                "-- Query active users\n" +
                "SELECT \n" +
                "    u.Id,\n" +
                "    u.UserName,\n" +
                "    u.Email,\n" +
                "    DATEDIFF(day, u.CreatedAt, GETUTCDATE()) AS DaysSinceCreated\n" +
                "FROM Users u\n" +
                "WHERE u.IsActive = 1\n" +
                "ORDER BY u.CreatedAt DESC;\n" +
                "```"
        });

        // Plain code block (no language)
        startPage.Blocks.Add(new MarkdownBlock
        {
            Body =
                "### Plain Text (No Language Specified)\n\n" +
                "```\n" +
                "This is a plain code block with no language identifier.\n" +
                "It should render as monospace text without syntax highlighting.\n" +
                "\n" +
                "Key features to test:\n" +
                "- Language class injection\n" +
                "- Alias normalization\n" +
                "- Prism.js initialization\n" +
                "- Multiple code blocks on one page\n" +
                "```"
        });

        await _api.Pages.SaveAsync(startPage);

        return Redirect("~/");
    }
}
