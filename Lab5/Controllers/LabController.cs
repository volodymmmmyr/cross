using Microsoft.AspNetCore.Mvc;
using ClassLibraryLab5;
public class LabController : Controller
{
    [HttpGet]
    public IActionResult Lab1()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Lab1Run(string inputValue)
    {
        string result = LibLab1.ExecuteLab1(inputValue);
        ViewBag.Result = result;
        return View("Lab1");
    }

    [HttpGet]
    public IActionResult Lab2()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Lab2Run(string inputValue)
    {
        string result = LibLab2.ExecuteLab2(inputValue);
        ViewBag.Result = result;
        return View("Lab2");
    }

    [HttpGet]
    public IActionResult Lab3()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Lab3Run(string inputValue)
    {
        string result = LibLab3.ExecuteLab3(inputValue);
        ViewBag.Result = result;
        return View("Lab3");
    }
}
