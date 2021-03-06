﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "update":
                    UpdateEmployee(employee);
                    break;

                case "read":
                    SelectEmployee(employee);
                    break;

                case "delete":
                    var employeeToBeDeleted = Query.CheckEmployeeToDelete(employee);
                    db.Employees.DeleteOnSubmit(employeeToBeDeleted);
                    db.SubmitChanges();
                    break;

                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
            }
        }
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }
        internal static Animal GetAnimalByID(int id)
        {
            var animal = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            return animal;
        }
        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            var animal = db.Animals.Where(a => a.AnimalId == animalId).FirstOrDefault();

            foreach (var update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        animal.Category.Name = updates[1];
                        break;
                    case 2:
                        animal.Name = updates[2];
                        break;
                    case 3:
                        animal.Age = Convert.ToInt32(updates[3]);
                        break;
                    case 4:
                        animal.Demeanor = updates[4];
                        break;
                    case 5:
                        animal.KidFriendly = Convert.ToBoolean(Convert.ToInt32(updates[5]));
                        break;
                    case 6:
                        animal.PetFriendly = Convert.ToBoolean(Convert.ToInt32(updates[6]));
                        break;
                    case 7:
                        animal.Weight = Convert.ToInt32(updates[7]);
                        break;
                    case 8:
                        break;
                }
            }
            db.SubmitChanges();
        }
        internal static void RemoveAnimal(Animal animal)
        {
            var deletedAnimal = db.Animals.Where(a => a == animal).FirstOrDefault();
            db.Animals.DeleteOnSubmit(deletedAnimal);
            db.SubmitChanges();
        }
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates)
        {
            var animal = db.Animals.Select(s => s);
            foreach (KeyValuePair<int,string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        animal = animal.Where(a => a.Category.Name == update.Value);
                        break;
                    case 2:
                        animal = animal.Where(a => a.Name == update.Value);
                        break;
                    case 3:
                        animal = animal.Where(a => a.Age == Convert.ToInt32(update.Value));
                        break;
                    case 4:
                        animal = animal.Where(a => a.Demeanor == update.Value);
                        break;
                    case 5:
                        animal = animal.Where(a => a.KidFriendly == Convert.ToBoolean(Convert.ToInt32(update.Value)));
                        break;
                    case 6:
                        animal = animal.Where(a => a.PetFriendly == Convert.ToBoolean(Convert.ToInt32(update.Value)));
                        break;
                    case 7:
                        animal = animal.Where(a => a.Weight == Convert.ToInt32(update.Value));
                        break;
                    case 8:
                        animal = animal.Where(a => a.AnimalId == Convert.ToInt32(update.Value));
                        break;
                    
                }
               
            }
            return animal;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var categoryID= db.Categories.Where(c => c.Name == categoryName).Select(i => i.CategoryId).FirstOrDefault();
                return categoryID;
        }
        
        internal static Room GetRoom(int animalId)
        {
            var room = db.Rooms.Where(r => r.AnimalId == animalId).Single();
            return room;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietPlanID = db.DietPlans.Where(d => d.Name == dietPlanName).Select(i => i.DietPlanId).FirstOrDefault();
            return dietPlanID;
        }
        internal static void Adopt(Animal animal, Client client)
        {
            var adoptedAnimal = db.Animals.Where(a => a.AnimalId == animal.AnimalId).FirstOrDefault();
            adoptedAnimal.AdoptionStatus = "Adoption Pending";
            var adoptionRow = db.Adoptions.FirstOrDefault();
            adoptionRow.ClientId = client.ClientId;
            adoptionRow.AnimalId = animal.AnimalId;
            adoptionRow.ApprovalStatus = "Pending";
            adoptionRow.AdoptionFee = 75;
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            var pendingAdoptions = db.Adoptions;
            pendingAdoptions.Where(a => a.ApprovalStatus == "Pending");
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (isAdopted)
            {
                adoption.ApprovalStatus = "Adopted";
                adoption.PaymentCollected = true;
                db.SubmitChanges();
            }
            else
            {
                adoption.ApprovalStatus = "Denied";
                var adoptedAnimal = db.Animals.Where(a => a.AnimalId == adoption.AnimalId).FirstOrDefault();
                adoptedAnimal.AdoptionStatus = "Available";
                db.SubmitChanges();
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            var removedAdoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(removedAdoption);
            db.SubmitChanges();
        }

        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var shots = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId);
            return shots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            var shotToUpdate = db.Shots.Where(s => s.Name == shotName).FirstOrDefault();
            var updatedShot = db.AnimalShots.FirstOrDefault();
            updatedShot.AnimalId = animal.AnimalId;
            updatedShot.ShotId = shotToUpdate.ShotId;
            updatedShot.DateReceived = DateTime.Now;
            db.AnimalShots.InsertOnSubmit(updatedShot);
            db.SubmitChanges();
        }
        internal static void UpdateEmployee(Employee employeeWithUpdates)
        {
            Employee employeeFromDB = null;

            try
            {
                employeeFromDB = db.Employees.Where(c => c.EmployeeId == employeeWithUpdates.EmployeeId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No employees have a EmployeeId that matches the Employee passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            employeeFromDB.FirstName = employeeWithUpdates.FirstName;
            employeeFromDB.LastName = employeeWithUpdates.LastName;
            employeeFromDB.EmployeeNumber = employeeWithUpdates.EmployeeNumber;
            employeeFromDB.Email = employeeWithUpdates.Email;

            db.SubmitChanges();
        }
        internal static void  SelectEmployee(Employee employeeToCheck)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeNumber == employeeToCheck.EmployeeNumber).FirstOrDefault();

            if(employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {

                Console.WriteLine(employeeFromDb.FirstName + " " + employeeFromDb.LastName + " " + employeeFromDb.UserName + " " +
                     " " + employeeFromDb.Email);
            }
        }
        internal static Employee CheckEmployeeToDelete(Employee employeeWithUpdates)
        {
            Employee employeeFromDB = null;

            try
            {
                employeeFromDB = db.Employees.Where(e => e.LastName == employeeWithUpdates.LastName && e.EmployeeNumber == employeeWithUpdates.EmployeeNumber).SingleOrDefault();
            }

            catch (InvalidOperationException e)
            {
                Console.WriteLine("No employees have a EmployeeId that matches the Employee passed in.");
                Console.WriteLine("No employee has been removed.");        

            }
            return employeeFromDB;
        }
    }
}