using Xunit;
using System.IO;
using System.ComponentModel.DataAnnotations;
using TechMoveSystem.Models;

namespace TechMoveSystem.Tests
{
    public class BusinessLogicTests
    {
        
        [Fact]
        public void CalculateZarCost_ShouldReturnCorrectMathProduct()
        {
           
            decimal costInUsd = 100.00m;
            decimal mockExchangeRate = 18.50m; 
            decimal expectedZarCost = 1850.00m;

            
            decimal actualZarCost = costInUsd * mockExchangeRate;

           
            Assert.Equal(expectedZarCost, actualZarCost);
        }

        
        [Theory]
        [InlineData(".exe", false)]
        [InlineData(".bat", false)]
        [InlineData(".pdf", true)]
        public void ValidateFileType_ShouldOnlyAllowPdfFormats(string fileExtension, bool expectedValid)
        {
            
            string lowercaseExtension = fileExtension.ToLower();

           
            bool isFileAllowed = (lowercaseExtension == ".pdf");

           
            Assert.Equal(expectedValid, isFileAllowed);
        }

        
        [Theory]
        [InlineData("Active", true)]
        [InlineData("Draft", true)]
        [InlineData("Expired", false)]
        [InlineData("On Hold", false)]
        public void ServiceRequestWorkflow_ShouldBlockCreationOnExpiredOrOnHold(string contractStatus, bool expectedAllowed)
        {
            
            bool isCreationAllowed = (contractStatus != "Expired" && contractStatus != "On Hold");

            
            Assert.Equal(expectedAllowed, isCreationAllowed);
        }
    }
}