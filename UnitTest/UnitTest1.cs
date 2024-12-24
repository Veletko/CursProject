using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using CursProgect;
using System.Diagnostics.Contracts;

namespace CursProgect.Tests
{
    [TestClass]
    public class SearchByDateTests
    {
        [TestMethod]
        public void SearchByDate_ValidDates_ReturnsFilteredContracts()
        {
            var contracts = new List<Contract>
            {
                new Contract { ContractNumber = 1, ContractDate = new DateTime(2023, 5, 1), SupplierCompany = "CompanyA", RawMaterial = "Material1", Country = "CountryA", ReceiverCompany = "ReceiverA", Amount = 100 },
                new Contract { ContractNumber = 2, ContractDate = new DateTime(2023, 6, 1), SupplierCompany = "CompanyB", RawMaterial = "Material2", Country = "CountryB", ReceiverCompany = "ReceiverB", Amount = 200 },
                new Contract { ContractNumber = 3, ContractDate = new DateTime(2023, 7, 1), SupplierCompany = "CompanyC", RawMaterial = "Material3", Country = "CountryC", ReceiverCompany = "ReceiverC", Amount = 300 }
            };

            using (var input = new StringReader("01.06.2023\n01.07.2023\n"))
            {
                Console.SetIn(input);
                var result = SearchByDate(contracts);
                Assert.AreEqual(2, result.Count);
                Assert.IsTrue(result.Exists(c => c.ContractNumber == 2)); // Проверяем, что контракт с номером 2 включен
                Assert.IsTrue(result.Exists(c => c.ContractNumber == 3)); // Проверяем, что контракт с номером 3 включен
            }
        }

        [TestMethod]
        public void SearchByDate_NoContractsInRange_ReturnsEmptyList()
        {
            var contracts = new List<Contract>
            {
                new Contract { ContractNumber = 1, ContractDate = new DateTime(2023, 5, 1), SupplierCompany = "CompanyA", RawMaterial = "Material1", Country = "CountryA", ReceiverCompany = "ReceiverA", Amount = 100 },
                new Contract { ContractNumber = 2, ContractDate = new DateTime(2023, 6, 1), SupplierCompany = "CompanyB", RawMaterial = "Material2", Country = "CountryB", ReceiverCompany = "ReceiverB", Amount = 200 }
            };

            using (var input = new StringReader("01.07.2023\n01.08.2023\n"))
            {
                Console.SetIn(input);
                var result = SearchByDate(contracts);
                Assert.AreEqual(0, result.Count); 
            }
        }
        private List<Contract> SearchByDate(List<Contract> contracts)
        {
            Console.WriteLine("Введите начальную дату (в формате dd.MM.yyyy):");
            DateTime startDate = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Введите конечную дату (в формате dd.MM.yyyy):");
            DateTime endDate = DateTime.Parse(Console.ReadLine());

            List<Contract> result = new List<Contract>();

            foreach (var contract in contracts)
            {
                if (contract.ContractDate >= startDate && contract.ContractDate <= endDate)
                {
                    result.Add(contract);
                }
            }
            return result;
        }
       
    }
    [TestClass]
    public class ReportTests
    {
        [TestMethod]
        public void ShowRawMaterialSummary_ValidData_CreatesCorrectSummary()
        {
            var contracts = new List<Contract>
            {
                new Contract { ContractNumber = 1, ContractDate = new DateTime(2023, 5, 1), SupplierCompany = "CompanyA", RawMaterial = "Material1", Country = "CountryA", ReceiverCompany = "ReceiverA", Amount = 100 },
                new Contract { ContractNumber = 2, ContractDate = new DateTime(2023, 6, 1), SupplierCompany = "CompanyB", RawMaterial = "Material2", Country = "CountryB", ReceiverCompany = "ReceiverB", Amount = 200 },
                new Contract { ContractNumber = 3, ContractDate = new DateTime(2023, 7, 1), SupplierCompany = "CompanyC", RawMaterial = "Material1", Country = "CountryC", ReceiverCompany = "ReceiverC", Amount = 300 }
            };

            string expectedReport = "Итоги по видам сырья\n" +
                                    "№ п/п\tВид сырья\tСумма\n" +
                                    "1\tMaterial1\t400\n" +
                                    "2\tMaterial2\t200\n" +
                                    "Итого:\t600\n";

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                ShowRawMaterialSummary(ref contracts);
                string result = sw.ToString();
                Assert.IsTrue(result.Contains(expectedReport));
            }
        }

        [TestMethod]
        public void ShowRawMaterialSummary_EmptyContracts_CreatesEmptySummary()
        {
            var contracts = new List<Contract>();
            string expectedReport = "Итоги по видам сырья\n" +
                                    "№ п/п\tВид сырья\tСумма\n" +
                                    "Итого:\t0\n";

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                ShowRawMaterialSummary(ref contracts);
                string result = sw.ToString();
                Assert.IsTrue(result.Contains(expectedReport));
            }
        }
        private void ShowRawMaterialSummary(ref List<Contract> contracts)
        {
            var summary = new Dictionary<string, int>();

            foreach (var contract in contracts)
            {
                if (summary.ContainsKey(contract.RawMaterial))
                {
                    summary[contract.RawMaterial] += contract.Amount;
                }
                else
                {
                    summary[contract.RawMaterial] = contract.Amount;
                }
            }

            string report = "Итоги по видам сырья\n";
            report += "№ п/п\tВид сырья\tСумма\n";
            int index = 1;
            foreach (var item in summary)
            {
                report += $"{index++}\t{item.Key}\t{item.Value}\n";
            }
            report += $"Итого:\t{summary.Values.Sum()}\n";

            Console.WriteLine(report);
        }
    }
}

