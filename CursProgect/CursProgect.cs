using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public struct Contract
{
    public int ContractNumber;
    public DateTime ContractDate;
    public string SupplierCompany;
    public string RawMaterial;
    public string Country;
    public string ReceiverCompany;
    public int Amount;
}

class CursProgect
{
    static string rawMaterialFile = "..\\..\\..\\rawMaterials.txt";
    static string countryFile = "..\\..\\..\\countries.txt";
    static string companyFile = "..\\..\\..\\companies.txt";
    static string pathForContractId = "..\\..\\..\\ContractLastID.txt";
    static string contractsFile = "..\\..\\..\\contracts.txt";

    static void Main()
    {
        List<Contract> contracts = LoadContractsFromFile(contractsFile);
        List<string> rawMaterials = LoadDataFromFile(rawMaterialFile);
        List<string> countries = LoadDataFromFile(countryFile);
        List<string> companies = LoadDataFromFile(companyFile);
        int ContractLastID = int.Parse(File.ReadLines(pathForContractId).First());
        MainMenu(ref contracts, ref companies, ref countries, ref rawMaterials,ref ContractLastID);
        WorkWithFilesMenu(ref contracts);
    }
    static DateTime ImputTime()
    {
        DateTime time;
        Console.WriteLine("Введите дату в формате dd.MM.yyyy");
        string input;
        do
        {
            input = Console.ReadLine();
        }
        while (!DateTime.TryParseExact(input, "dd.MM.yyyy", null, DateTimeStyles.None, out time));
        DateTime truncatedTime = new DateTime(time.Year, time.Month, time.Day);
        return truncatedTime;
    }
    static void CleanFile(string path)
    {
        if (File.Exists(path))
        {
            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                fileStream.SetLength(0);
            }
        }
    }
    static void AddToFile(string text, string path)
    {
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(text);
        }
    }
    static bool CheckIdInput(string idInput)
    {
        if (int.TryParse(idInput, out int idInt) && idInt > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    static bool CheckEmployeeExistence(List<Contract> contracts, int id)
    {
        return contracts.Any(e => e.ContractNumber == id);
    }
    static int FullCheckContractId(ref List<Contract> contract)
    {
        string idInput;
        int id = 0;

        while (true)
        {
            idInput = Console.ReadLine();
            if (CheckIdInput(idInput) && int.TryParse(idInput, out id) && CheckEmployeeExistence(contract, id))
            {
                return id;
            }
            Console.WriteLine("Нет такого контракта. Повторите ввод.");
        }
    }
    static void WriteContracts(ref List<Contract> contracts)
    {
        for (int i = 0; i < contracts.Count; i++)
        {
            Console.WriteLine($"{contracts[i].ContractNumber} {contracts[i].SupplierCompany} {contracts[i].ReceiverCompany} " +
                $"{contracts[i].RawMaterial} {contracts[i].Amount} {contracts[i].ContractDate}");
        }
    }
    static string ChoseRawMaterial(List<string> rawMaterials)
    {
        Console.WriteLine("Существующие виды сырья:");
        for (int i = 0; i < rawMaterials.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {rawMaterials[i]}");
        }

        Console.WriteLine("Выберите вид сырья по номеру в списке:");

        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice > 0 && choice <= rawMaterials.Count)
                {
                    return rawMaterials[choice - 1];
                }
            }

            Console.WriteLine("Ошибка: введите корректный номер из списка.");
        }
    }
    static string ChoseCountry(List<string> countries)
    {
        Console.WriteLine("Существующие страны:");
        for (int i = 0; i < countries.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {countries[i]}");
        }

        Console.WriteLine("Выберите страну по номеру в списке:");

        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice > 0 && choice <= countries.Count)
                {
                    return countries[choice - 1];
                }
            }

            Console.WriteLine("Ошибка: введите корректный номер из списка.");
        }
    }
    static string ChoseCompany(List<string> componies)
    {
        Console.WriteLine("Существующие компании:");
        for (int i = 0; i < componies.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {componies[i]}");
        }

        Console.WriteLine("Выберите компанию по номеру в списке:");

        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice > 0 && choice <= componies.Count)
                {
                    return componies[choice - 1];
                }
            }

            Console.WriteLine("Ошибка: введите корректный номер из списка.");
        }
    }
    static Contract CreateContract(List<string> rawMaterials, List<string> countries, List<string> companies)
    {
        Contract contract = new Contract();
        contract.ContractDate = ImputTime();
        Console.WriteLine("Выберите поставщика:");
        contract.SupplierCompany = ChoseCompany(companies);
        Console.WriteLine("Выберите получателя:");
        while (true)
        {
            contract.ReceiverCompany = ChoseCompany(companies);
            if (contract.ReceiverCompany != contract.SupplierCompany)
            {
                break;
            }
            Console.WriteLine("Ошибка: компания получатель не должна совпадать с компанией поставщиком. Повторите выбор.");
        }
        Console.WriteLine("Выберите материал");
        contract.RawMaterial = ChoseRawMaterial(rawMaterials);
        Console.WriteLine("Выберите страну");
        contract.Country = ChoseCountry(countries);
        Console.WriteLine("Выберите количество");
        contract.Amount = int.Parse(Console.ReadLine());

        return contract;
    }
    static List<string> LoadDataFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            File.Create(fileName).Close();
            return new List<string>();
        }

        return File.ReadAllLines(fileName).ToList();
    }
    static void SaveDataToFile(string fileName, List<string> data)
    {
        File.WriteAllLines(fileName, data);
    }
    static void ManageRawMaterials(List<string> rawMaterials)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Добавить новый вид сырья");
            Console.WriteLine("2. Удалить вид сырья");
            Console.WriteLine("3. Изменить вид сырья");
            Console.WriteLine("4. Показать все виды сырья");
            Console.WriteLine("0. Возврат в главное меню");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > 4)
            {
                Console.WriteLine("Неверный выбор. Попробуйте снова.");
                Console.ReadKey();
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.WriteLine("Введите новый вид сырья:");
                    string newRawMaterial = Console.ReadLine();
                    rawMaterials.Add(newRawMaterial);
                    SaveDataToFile(rawMaterialFile, rawMaterials);
                    Console.WriteLine("Вид сырья добавлен.");
                    break;

                case 2:
                    Console.WriteLine("Выберите номер вида сырья для удаления:");
                    for (int i = 0; i < rawMaterials.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {rawMaterials[i]}");
                    }

                    if (int.TryParse(Console.ReadLine(), out int removeIndex) && removeIndex > 0 && removeIndex <= rawMaterials.Count)
                    {
                        rawMaterials.RemoveAt(removeIndex - 1);
                        SaveDataToFile(rawMaterialFile, rawMaterials);
                        Console.WriteLine("Вид сырья удален.");
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    }
                    break;

                case 3:
                    Console.WriteLine("Выберите номер вида сырья для изменения:");
                    for (int i = 0; i < rawMaterials.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {rawMaterials[i]}");
                    }

                    if (int.TryParse(Console.ReadLine(), out int editIndex) && editIndex > 0 && editIndex <= rawMaterials.Count)
                    {
                        Console.WriteLine("Введите новый вид сырья:");
                        string updatedRawMaterial = Console.ReadLine();
                        rawMaterials[editIndex - 1] = updatedRawMaterial;
                        SaveDataToFile(rawMaterialFile, rawMaterials);
                        Console.WriteLine("Вид сырья изменен.");
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    }
                    break;

                case 4:
                    Console.WriteLine("Существующие виды сырья:");
                    for (int i = 0; i < rawMaterials.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {rawMaterials[i]}");
                    }
                    break;

                case 0:
                    return;
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
    static void ManageCountries(List<string> countries)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Добавить новую страну");
            Console.WriteLine("2. Удалить страну");
            Console.WriteLine("3. Изменить страну");
            Console.WriteLine("4. Показать все страны");
            Console.WriteLine("0. Выход");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > 4)
            {
                Console.WriteLine("Неверный выбор. Попробуйте снова.");
                Console.ReadKey();
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.WriteLine("Введите новую страну:");
                    string newCountry = Console.ReadLine();
                    countries.Add(newCountry);
                    SaveDataToFile(countryFile, countries);
                    Console.WriteLine("Страна добавлена.");
                    break;

                case 2:
                    Console.WriteLine("Выберите номер страны для удаления:");
                    for (int i = 0; i < countries.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {countries[i]}");
                    }

                    if (int.TryParse(Console.ReadLine(), out int removeIndex) && removeIndex > 0 && removeIndex <= countries.Count)
                    {
                        countries.RemoveAt(removeIndex - 1);
                        SaveDataToFile(countryFile, countries);
                        Console.WriteLine("Страна удалена.");
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    }
                    break;

                case 3:
                    Console.WriteLine("Выберите номер страны для изменения:");
                    for (int i = 0; i < countries.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {countries[i]}");
                    }

                    if (int.TryParse(Console.ReadLine(), out int editIndex) && editIndex > 0 && editIndex <= countries.Count)
                    {
                        Console.WriteLine("Введите новую страну:");
                        string updatedCountry = Console.ReadLine();
                        countries[editIndex - 1] = updatedCountry;
                        SaveDataToFile(countryFile, countries);
                        Console.WriteLine("Страна изменена.");
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    }
                    break;

                case 4:
                    Console.WriteLine("Существующие страны:");
                    for (int i = 0; i < countries.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {countries[i]}");
                    }
                    break;

                case 0:
                    return;
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
    static void ManageCompanies(List<string> companies)
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("1. Добавить новую компанию");
            Console.WriteLine("2. Удалить компанию");
            Console.WriteLine("3. Изменить название компании");
            Console.WriteLine("4. Показать все компании");
            Console.WriteLine("0. Выход");

            Console.Write("\nВведите номер действия: ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Ошибка ввода! Введите число.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.WriteLine("Введите название новой компании:");
                    string newCompany = Console.ReadLine();
                    companies.Add(newCompany);
                    SaveDataToFile(companyFile, companies);
                    Console.WriteLine("Компания добавлена.");
                    break;

                case 2:
                    Console.WriteLine("Список компаний:");
                    for (int i = 0; i < companies.Count; i++)
                        Console.WriteLine($"{i + 1}. {companies[i]}");

                    Console.Write("\nВведите номер компании для удаления: ");
                    if (int.TryParse(Console.ReadLine(), out int removeIndex) && removeIndex > 0 && removeIndex <= companies.Count)
                    {
                        Console.WriteLine($"Компания \"{companies[removeIndex - 1]}\" удалена.");
                        companies.RemoveAt(removeIndex - 1);
                        SaveDataToFile(companyFile, companies);
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: неверный номер компании.");
                    }
                    break;

                case 3:
                    Console.WriteLine("Список компаний:");
                    for (int i = 0; i < companies.Count; i++)
                        Console.WriteLine($"{i + 1}. {companies[i]}");

                    Console.Write("\nВведите номер компании для изменения: ");
                    if (int.TryParse(Console.ReadLine(), out int editIndex) && editIndex > 0 && editIndex <= companies.Count)
                    {
                        Console.Write($"Введите новое название для компании \"{companies[editIndex - 1]}\": ");
                        string updatedCompany = Console.ReadLine();
                        companies[editIndex - 1] = updatedCompany;
                        SaveDataToFile(companyFile, companies);
                        Console.WriteLine("Название компании изменено.");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: неверный номер компании.");
                    }
                    break;

                case 4:
                    Console.WriteLine("Список компаний:");
                    foreach (var company in companies)
                        Console.WriteLine(company);
                    break;

                case 0:
                    return;

                default:
                    Console.WriteLine("Ошибка: такой опции нет. Попробуйте снова.");
                    break;
            }
        }
    }
    static void AddContractToList(ref List<Contract> contracts,ref List<string> rawMaterials,ref List<string> countries,ref List<string> companies, ref int contractId)
    {
        Contract newContract = CreateContract(rawMaterials,countries,companies);
        newContract.ContractNumber = ++contractId;
        contracts.Add(newContract);

        CleanFile(pathForContractId);
        AddToFile(contractId.ToString(), pathForContractId);
    }
    static void DeleteContract(ref List<Contract> contracts, int contractId)
    {
        var contractToDelete = contracts.FirstOrDefault(c => c.ContractNumber == contractId);
        if (contractToDelete.Equals(default(Contract)))
        {
            Console.WriteLine("Контракт с указанным ID не найден.");
            return;
        }
        contracts.Remove(contractToDelete);
        Console.WriteLine($"Контракт с ID {contractId} успешно удален.");
    }
    static void SortByDate(ref List<Contract> contracts)
    {
        for (int i = 0; i < contracts.Count - 1; i++)
        {
            for (int j = 0; j < contracts.Count - i - 1; j++)
            {
                if (contracts[j].ContractDate > contracts[j + 1].ContractDate)
                {
                    var temp = contracts[j];
                    contracts[j] = contracts[j + 1];
                    contracts[j + 1] = temp;
                }
            }
        }
        Console.WriteLine("Контракты отсортированы по дате заключения.");
    }
    static void SortBySupplierCompany(ref List<Contract> contracts)
    {
        for (int i = 0; i < contracts.Count - 1; i++)
        {
            for (int j = 0; j < contracts.Count - i - 1; j++)
            {
                if (string.Compare(contracts[j].SupplierCompany, contracts[j + 1].SupplierCompany) > 0)
                {
                    var temp = contracts[j];
                    contracts[j] = contracts[j + 1];
                    contracts[j + 1] = temp;
                }
            }
        }
        Console.WriteLine("Контракты отсортированы по фирме-поставщику.");
    }
    static void SortByCountry(ref List<Contract> contracts)
    {
        for (int i = 0; i < contracts.Count - 1; i++)
        {
            for (int j = 0; j < contracts.Count - i - 1; j++)
            {
                if (string.Compare(contracts[j].Country, contracts[j + 1].Country) > 0)
                {
                    var temp = contracts[j];
                    contracts[j] = contracts[j + 1];
                    contracts[j + 1] = temp;
                }
            }
        }
        Console.WriteLine("Контракты отсортированы по стране.");
    }
    static void SortByReceiverCompany(ref List<Contract> contracts)
    {
        for (int i = 0; i < contracts.Count - 1; i++)
        {
            for (int j = 0; j < contracts.Count - i - 1; j++)
            {
                if (string.Compare(contracts[j].ReceiverCompany, contracts[j + 1].ReceiverCompany) > 0)
                {
                    var temp = contracts[j];
                    contracts[j] = contracts[j + 1];
                    contracts[j + 1] = temp;
                }
            }
        }
        Console.WriteLine("Контракты отсортированы по фирме-получателю.");
    }
    static void SortByRawMaterial(ref List<Contract> contracts)
    {
        for (int i = 0; i < contracts.Count - 1; i++)
        {
            for (int j = 0; j < contracts.Count - i - 1; j++)
            {
                if (string.Compare(contracts[j].RawMaterial, contracts[j + 1].RawMaterial) > 0)
                {
                    var temp = contracts[j];
                    contracts[j] = contracts[j + 1];
                    contracts[j + 1] = temp;
                }
            }
        }
        Console.WriteLine("Контракты отсортированы по виду сырья.");
    }
    static void ChangeContract(ref List<Contract> contracts, int changeId, ref List<string> companies,ref List<string> countries, ref List<string> rawMaterials)
    {
        var contract = contracts.FirstOrDefault(c => c.ContractNumber == changeId);
        if (contract.ContractNumber == 0) 
        {
            Console.WriteLine("Контракт с указанным ID не найден.");
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выбран контракт:");
            Console.WriteLine($"ID: {contract.ContractNumber}, Дата: {contract.ContractDate}, Компания: {contract.SupplierCompany}, " +
                              $"Сырье: {contract.RawMaterial}, Страна: {contract.Country}, " +
                              $"Получатель: {contract.ReceiverCompany}, Количество: {contract.Amount}");
            Console.WriteLine("\nЧто вы хотите изменить?");
            Console.WriteLine("1 - Дата контракта");
            Console.WriteLine("2 - Компания поставщик");
            Console.WriteLine("3 - Вид сырья");
            Console.WriteLine("4 - Страна");
            Console.WriteLine("5 - Компания-получатель");
            Console.WriteLine("6 - Количество");
            Console.WriteLine("7 - Вернуться");

            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    contract.ContractDate = ImputTime();
                    break;
                case "2":
                    Console.Write("Введите новую компанию поставщика: ");
                    contract.SupplierCompany = ChoseCompany(companies);
                    break;
                case "3":
                    Console.Write("Введите новый вид сырья: ");
                    contract.RawMaterial = ChoseRawMaterial(rawMaterials);
                    break;
                case "4":
                    Console.Write("Введите новую страну: ");
                    contract.Country = ChoseCountry(countries);
                    break;
                case "5":
                    Console.WriteLine("Выберите получателя:");
                    while (true)
                    {
                        contract.ReceiverCompany = ChoseCompany(companies);
                        if (contract.ReceiverCompany != contract.SupplierCompany)
                        {
                            break;
                        }
                        Console.WriteLine("Ошибка: компания получатель не должна совпадать с компанией поставщиком. Повторите выбор.");
                    }
                    break;
                case "6":
                    Console.Write("Введите новое количество: ");
                    if (int.TryParse(Console.ReadLine(), out int newAmount))
                    {
                        contract.Amount = newAmount;
                    }
                    else
                    {
                        Console.WriteLine("Неверное значение количества!");
                    }
                    break;
                case "7":
                    return; // Выход из подменю
                default:
                    Console.WriteLine("Неверный выбор, попробуйте еще раз.");
                    break;
            }

            // Сохраняем изменения в списке
            var index = contracts.FindIndex(c => c.ContractNumber == changeId);
            if (index != -1)
            {
                contracts[index] = contract;
            }
        }
    }
    static void MainMenu(ref List<Contract> contracts, ref List<string> companies, ref List<string> countries, ref List<string> rawMaterials, ref int contractId)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1 - редактирование данных\r\n" +
                              "2 - поиск информации\r\n" +
                              "3 - сортировка данных\r\n" +
                              "4 - создание отчетов\r\n" +
                              "5 - выход из программы");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    EditingMenu(ref contracts, ref rawMaterials, ref countries, ref companies, ref contractId);
                    break;
                case "2":
                    SearchContractsMenu(contracts);
                    break;
                case "3":
                    SortingMenu(ref contracts, ref rawMaterials, ref countries, ref companies, ref contractId);
                    break;
                case "4":
                    ReportMenu(ref contracts, ref rawMaterials, ref countries, ref companies);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
        }
    }
    static void EditingMenu(ref List<Contract> contracts, ref List<string> rawMaterials, ref List<string> countries, ref List<string> companies, ref int contractId)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1 - добавление договора\r\n" +
                       "2 - изменение данных о договоре\r\n" +
                       "3 - удаление договора\r\n" +
                       "4 - менеджер справочника видов сырья\r\n" +
                       "5 - менеджер справочника стран\r\n" +
                       "6 - менеджер справочника компаний\r\n" +
                       "7 - вернуться в основное меню");
            string choice = Console.ReadLine();
            if (choice == "7")
            {
                break;
            }
            switch (choice)
            {
                case "1":
                    AddContractToList(ref contracts,ref rawMaterials,ref countries,ref companies,ref contractId);
                    break;
                case "2":
                    Console.WriteLine("Все контракты:");
                    WriteContracts(ref contracts);
                    Console.WriteLine("введиет ID контракта, который хотите изменить");
                    int changeId = FullCheckContractId(ref contracts);
                    ChangeContract(ref contracts, changeId, ref companies, ref countries, ref rawMaterials);
                    break;
                case "3":
                    Console.WriteLine("Все контракты:");
                    WriteContracts(ref contracts);
                    Console.WriteLine("введиет ID контракта, который хотите удалить");
                    int deleteId = FullCheckContractId(ref contracts);
                    DeleteContract(ref contracts, deleteId);
                    break;
                case "4":
                    ManageRawMaterials(rawMaterials);
                    break;
                case "5":
                    ManageCountries(countries);
                    return;
                case "6":
                    ManageCompanies(companies);
                    return;
            }
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
    static void SortingMenu(ref List<Contract> contracts, ref List<string> rawMaterials, ref List<string> countries, ref List<string> companies, ref int contractId)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1 - cортировка по дате заключения договора\r\n" +
                "2 - cортировка по фирме-поставщику\r\n" +
                "3 - cортировка по стране\r\n" +
                "4 - cортировка по фирме - получателю\r\n" +
                "5 - cортировка по виду сырья\r\n" +
                "6 - вернуться в главное меню");
            string choice = Console.ReadLine();
            if (choice == "6")
            {
                break;
            }
            switch (choice)
            {
                case "1":
                    SortByDate(ref contracts);
                    break;
                case "2":
                    SortBySupplierCompany(ref contracts);      
                    break;
                case "3":
                    SortByCountry(ref contracts);
                    break;
                case "4":
                    SortByReceiverCompany(ref contracts);
                    break;
                case "5":
                    SortByRawMaterial(ref contracts);
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                    break;
            }
            WriteContracts(ref contracts);
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
    static void SearchContractsMenu(List<Contract> contracts)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите критерий поиска:\r\n" +
                              "1 - Поиск по дате заключения договора\r\n" +
                              "2 - Поиск по фирме-поставщику\r\n" +
                              "3 - Поиск по виду сырья\r\n" +
                              "4 - Поиск по стране-экспортеру\r\n" +
                              "5 - Вернуться в главное меню");
            string choice = Console.ReadLine();
            if (choice == "5")
            {
                break;
            }

            List<Contract> filteredContracts = new List<Contract>();

            switch (choice)
            {
                case "1":
                    filteredContracts = SearchByDate(contracts);
                    break;

                case "2":
                    filteredContracts = SearchBySupplier(contracts);
                    break;

                case "3":
                    filteredContracts = SearchByRawMaterial(contracts);
                    break;

                case "4":
                    filteredContracts = SearchByCountry(contracts);
                    break;

                default:
                    Console.WriteLine("Некорректный выбор, попробуйте снова.");
                    continue;
            }
            DisplaySearchResults(filteredContracts);
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
    static List<Contract> SearchByDate(List<Contract> contracts)
    {
        Console.WriteLine("Введите начальную дату (в формате dd.MM.yyyy):");
        DateTime startDate = ImputTime();
        Console.WriteLine("Введите конечную дату (в формате dd.MM.yyyy):");
        DateTime endDate = ImputTime();

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
    static List<Contract> SearchBySupplier(List<Contract> contracts)
    {
        Console.WriteLine("Введите название фирмы-поставщика:");
        string supplier = Console.ReadLine();

        List<Contract> result = new List<Contract>();

        foreach (var contract in contracts)
        {
            if (contract.SupplierCompany.Equals(supplier, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(contract);
            }
        }
        return result;
    }
    static List<Contract> SearchByRawMaterial(List<Contract> contracts)
    {
        Console.WriteLine("Введите вид сырья:");
        string rawMaterial = Console.ReadLine();

        List<Contract> result = new List<Contract>();

        foreach (var contract in contracts)
        {
            if (contract.RawMaterial.Equals(rawMaterial, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(contract);
            }
        }
        return result;
    }
    static List<Contract> SearchByCountry(List<Contract> contracts)
    {
        Console.WriteLine("Введите страну-экспортера:");
        string country = Console.ReadLine();

        List<Contract> result = new List<Contract>();

        foreach (var contract in contracts)
        {
            if (contract.Country.Equals(country, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(contract);
            }
        }
        return result;
    }
    static void DisplaySearchResults(List<Contract> contracts)
    {
        Console.WriteLine("\nРезультаты поиска:");
        if (contracts.Count == 0)
        {
            Console.WriteLine("Договоры не найдены по заданным критериям.");
        }
        else
        {
            foreach (var contract in contracts)
            {
                Console.WriteLine($"ID: {contract.ContractNumber}, Поставщик: {contract.SupplierCompany}, Получатель: {contract.ReceiverCompany}, " +
                                  $"Сырье: {contract.RawMaterial}, Страна: {contract.Country}, Дата: {contract.ContractDate.ToShortDateString()}, " +
                                  $"Количество: {contract.Amount}");
            }
        }
    }
    static void ReportMenu(ref List<Contract> contracts, ref List<string> rawMaterials, ref List<string> countries, ref List<string> companies)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите пункт меню:");
            Console.WriteLine("1. Генерация отчета по видам сырья");
            Console.WriteLine("2. Генерация отчета по странам и видам сырья");
            Console.WriteLine("3. Генерация отчета по поставщикам и странам");
            Console.WriteLine("4. Генерация отчета по поставщикам за месяц");
            Console.WriteLine("5. Возврат в главное меню");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ShowRawMaterialSummary(ref contracts);
                    break;
                case "2":
                    ShowCountryRawMaterialSummary(ref contracts);
                    break;
                case "3":
                    ShowSupplierCountrySummary(ref contracts);
                    break;
                case "4":
                    ShowSuppliersByMonth(ref contracts);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
    }
    static void SaveReportToFile(string reportContent, string reportName)
    {
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = Path.Combine(directoryPath, $"{reportName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        File.WriteAllText(filePath, reportContent);
    }
    static void ShowRawMaterialSummary(ref List<Contract> contracts)
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

        SaveReportToFile(report, "RawMaterialSummary");
    }
    static void ShowCountryRawMaterialSummary(ref List<Contract> contracts)
    {
        var summary = new Dictionary<string, Dictionary<string, int>>();

        foreach (var contract in contracts)
        {
            if (!summary.ContainsKey(contract.Country))
            {
                summary[contract.Country] = new Dictionary<string, int>();
            }

            if (summary[contract.Country].ContainsKey(contract.RawMaterial))
            {
                summary[contract.Country][contract.RawMaterial] += contract.Amount;
            }
            else
            {
                summary[contract.Country][contract.RawMaterial] = contract.Amount;
            }
        }

        string report = "Итоги по странам и видам сырья\n<Страна>\n";
        report += "№ п/п\tВид сырья\tСумма\n";
        int index = 1;
        foreach (var country in summary)
        {
            report += $"\n{country.Key}\n";
            foreach (var item in country.Value)
            {
                report += $"{index++}\t{item.Key}\t{item.Value}\n";
            }
        }
        int totalAmount = 0;
        foreach (var country in summary)
        {
            foreach (var item in country.Value)
            {
                totalAmount += item.Value;
            }
        }
        report += $"Итого:\t{totalAmount}\n";

        SaveReportToFile(report, "CountryRawMaterialSummary");
    }
    static void ShowSupplierCountrySummary(ref List<Contract> contracts)
    {
        var summary = new Dictionary<string, Dictionary<string, int>>();

        foreach (var contract in contracts)
        {
            if (!summary.ContainsKey(contract.Country))
            {
                summary[contract.Country] = new Dictionary<string, int>();
            }

            if (summary[contract.Country].ContainsKey(contract.SupplierCompany))
            {
                summary[contract.Country][contract.SupplierCompany] += contract.Amount;
            }
            else
            {
                summary[contract.Country][contract.SupplierCompany] = contract.Amount;
            }
        }

        string report = "Итоги по фирмам-поставщикам и странам\n<Страна>\n";
        report += "№ п/п\tПоставщик\tСумма\n";
        int index = 1;
        foreach (var country in summary)
        {
            report += $"\n{country.Key}\n";
            foreach (var item in country.Value)
            {
                report += $"{index++}\t{item.Key}\t{item.Value}\n";
            }
        }
        int totalAmount = 0;
        foreach (var country in summary)
        {
            foreach (var item in country.Value)
            {
                totalAmount += item.Value;
            }
        }
        report += $"Итого:\t{totalAmount}\n";

        SaveReportToFile(report, "SupplierCountrySummary");
    }
    static void ShowSuppliersByMonth(ref List<Contract> contracts)
    {
        var summary = new Dictionary<string, Dictionary<string, List<int>>>();

        foreach (var contract in contracts)
        {
            string monthYear = $"{contract.ContractDate.Month}/{contract.ContractDate.Year}";

            if (!summary.ContainsKey(monthYear))
            {
                summary[monthYear] = new Dictionary<string, List<int>>();
            }

            if (!summary[monthYear].ContainsKey(contract.SupplierCompany))
            {
                summary[monthYear][contract.SupplierCompany] = new List<int>();
            }

            summary[monthYear][contract.SupplierCompany].Add(contract.Amount);
        }

        string report = "Список поставщиков по месяцам\n";
        report += "№ п/п\tПоставщик\tСумма\tМесяц\n";
        int index = 1;
        foreach (var monthYear in summary)
        {
            foreach (var supplier in monthYear.Value)
            {
                int totalAmount = 0;
                foreach (var amount in supplier.Value)
                {
                    totalAmount += amount;
                }
                report += $"{index++}\t{supplier.Key}\t{totalAmount}\t{monthYear.Key}\n";
            }
        }

        SaveReportToFile(report, "SuppliersByMonth");
    }
    static void WorkWithFilesMenu(ref List<Contract> contracts)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1 - сохранить все изменения\r\n" +
                              "2 - выход");
            string choice = Console.ReadLine();
            if (choice == "2")
            {
                break;
            }
            switch (choice)
            {
                case "1":
                    SaveContractsToFile(contracts, contractsFile);
                    Console.WriteLine("Изменения сохранены. Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    static void SaveContractsToFile(List<Contract> contracts, string filePath)
    {
        var lines = contracts.Select(contract =>
            string.Join(",",
                contract.ContractNumber,
                contract.ContractDate.ToString("yyyy-MM-dd"),
                contract.SupplierCompany,
                contract.RawMaterial,
                contract.Country,
                contract.ReceiverCompany,
                contract.Amount));

        File.WriteAllLines(filePath, lines);
    }
    static List<Contract> LoadContractsFromFile(string filePath)
    {
        var contracts = new List<Contract>();
        if (!File.Exists(filePath))
        {
            return contracts;
        }

        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length == 7 &&
                int.TryParse(parts[0], out int contractNumber) &&
                DateTime.TryParse(parts[1], out DateTime contractDate) &&
                int.TryParse(parts[6], out int amount))
            {
                contracts.Add(new Contract
                {
                    ContractNumber = contractNumber,
                    ContractDate = contractDate,
                    SupplierCompany = parts[2],
                    RawMaterial = parts[3],
                    Country = parts[4],
                    ReceiverCompany = parts[5],
                    Amount = amount
                });
            }
        }

        return contracts;
    }

}
