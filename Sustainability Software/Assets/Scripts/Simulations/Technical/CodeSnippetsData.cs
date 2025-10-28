using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CodeSnippetsData", menuName = "Simulation/CodeSnippets")]
public class CodeSnippetsData : ScriptableObject
{
    public List<CodeSnippet> snippets = new List<CodeSnippet>
    {
        new CodeSnippet
        {
            codeText = "def calculate_area(width, height):\n    \"\"\"Returns the area of a rectangle.\"\"\"\n    return width * height",
            isGoodCode = true,
            feedbackGood = "Maintainable: clear function name, parameters, and documentation.",
            feedbackBad = "This is actually good code. Check function clarity and documentation."
        },
        new CodeSnippet
        {
            codeText = "def calc(a,b):\n    return a*b  # no comments, unclear variable names",
            isGoodCode = false,
            feedbackGood = "This is bad code: unclear naming and no documentation.",
            feedbackBad = "This code is hard to maintain: unclear logic and variable names."
        },
        new CodeSnippet
        {
            codeText = "class User:\n    def __init__(self, name, email):\n        self.name = name\n        self.email = email\n    \n    def display_info(self):\n        print(f'Name: {self.name}, Email: {self.email}')",
            isGoodCode = true,
            feedbackGood = "Well-structured class with clear methods and readable code.",
            feedbackBad = "This code is actually well-written: classes and methods are clear."
        },
        new CodeSnippet
        {
            codeText = "class U:\n    def __init__(self, n, e):\n        self.n = n\n        self.e = e\n    def d(self): print(self.n, self.e)",
            isGoodCode = false,
            feedbackGood = "Bad code: unclear class and variable names, no documentation.",
            feedbackBad = "Hard to maintain: unclear naming and minimal structure."
        },
        new CodeSnippet
        {
            codeText = "def fetch_data_from_api(url):\n    response = requests.get(url)\n    if response.status_code == 200:\n        return response.json()\n    else:\n        return None",
            isGoodCode = true,
            feedbackGood = "Clear, modular function with proper error handling.",
            feedbackBad = "This is actually maintainable: it’s readable and structured."
        },
        new CodeSnippet
        {
            codeText = "def getData(u):\n    r = requests.get(u)\n    return r.json()",
            isGoodCode = false,
            feedbackGood = "Bad code: poor naming, no error handling, not modular.",
            feedbackBad = "This code is fragile: unclear and lacks proper handling."
        }
    };
}