using System.Linq;
using System.Web.Mvc;
using BO;
using TP_pizzas.Models;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace TP_pizzas.Controllers
{

    public class PizzaController : Controller
    {
        private static List<Ingredient> listIngredients = Pizza.IngredientsDisponibles;
        private static List<Pate> listPates = Pizza.PatesDisponibles;

        private static List<Pizza> pizzas;

        public PizzaController()
        {
            if (pizzas == null)
            {
                pizzas = FakeDbPizza.Instance.Pizzas;
            }
        }

        // GET: Pizza
        public ActionResult Index()
        {
            return View(pizzas);
        }

        // GET: Pizza/Details/5
        public ActionResult Details(int id)
        {
            Pizza pizza = pizzas.FirstOrDefault(p => p.Id == id);
            if (pizza != null)
            {
                return View(pizza);
            }
            return RedirectToAction("Index");
        }

        // GET: Pizza/Create
        public ActionResult Create()
        {
            PizzaVm pizzaVM = new PizzaVm();
            pizzaVM.setIngredients(listIngredients);
            pizzaVM.setPates(listPates);
            return View(pizzaVM);
        }

        // POST: Pizza/Create
        [HttpPost]
        public ActionResult Create(PizzaVm pizzaVM)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (pizzaVM.selectedPate == 0)
                    {
                        throw new Exception("Une pizza doit toujours avoir une pâte.");
                    }

                    if (pizzaVM.selectedIngredients.Count() < 2 || pizzaVM.selectedIngredients.Count() > 5)
                    {
                        throw new Exception("Une pizza doit avoir entre 2 et 5 ingrédients.");
                    }
                    Pizza nouvellePizza = pizzaVM.Pizza;

                    if (pizzas.Any(p => p.Nom.ToUpper() == nouvellePizza.Nom.ToUpper() && nouvellePizza.Id != p.Id))
                    {
                        throw new Exception("Il existe déjà une pizza portant ce nom.");
                    }

                    nouvellePizza.Pate = listPates.FirstOrDefault(p => p.Id == pizzaVM.selectedPate);
                    foreach (var ingredient in pizzaVM.selectedIngredients)
                    {
                        nouvellePizza.Ingredients.Add(listIngredients.FirstOrDefault(i => i.Id == ingredient));
                    }

                    foreach (Pizza pizza in pizzas)
                    {
                        if (pizzaVM.selectedIngredients.Count() == nouvellePizza.Ingredients.Count())
                        {
                            List<Ingredient> pizzaBd = nouvellePizza.Ingredients.OrderBy(x => x.Id).ToList();
                            pizzaVM.selectedIngredients = pizzaVM.selectedIngredients.OrderBy(x => x).ToList();
                            bool isDifferent = false;
                            for (int i = 0; i < pizzaVM.selectedIngredients.Count(); i++)
                            {
                                if (pizzaVM.selectedIngredients.ElementAt(i) != pizzaBd.ElementAt(i).Id)
                                {
                                    isDifferent = true;
                                    break;
                                }
                            }
                            if (!isDifferent)
                            {
                                throw new Exception("Il existe une pizza avec ces ingrédients.");
                            }
                        }
                    }

                    int maxId = pizzas.Max(p => p.Id);
                    nouvellePizza.Id = maxId + 1;
                    pizzas.Add(nouvellePizza);
                    return RedirectToAction("Index");
                }
                throw new Exception("Formulaire invalide");

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                pizzaVM.setIngredients(listIngredients);
                pizzaVM.setPates(listPates);
                return View(pizzaVM);
            }
        }

        // GET: Pizza/Edit/5
        public ActionResult Edit(int id)
        {
            Pizza pizza = pizzas.FirstOrDefault(p => p.Id == id);
            if (pizza != null)
            {
                var pizzaVM = new PizzaVm();
                pizzaVM.setIngredients(listIngredients);
                pizzaVM.setPates(listPates);
                pizzaVM.Pizza = pizza;
                if (pizzaVM.Pizza.Pate != null)
                {
                    pizzaVM.selectedPate = pizzaVM.Pizza.Pate.Id;
                }

                if (pizzaVM.Pizza.Ingredients.Any())
                {
                    pizzaVM.selectedIngredients = pizzaVM.Pizza.Ingredients.Select(i => i.Id).ToList();
                }

                return View(pizzaVM);
            }
            return RedirectToAction("Index");
        }

        // POST: Pizza/Edit/5
        [HttpPost]
        public ActionResult Edit(PizzaVm pizzaVM)
        {
            try
            {
                if (pizzaVM.selectedPate == 0)
                {
                    throw new Exception("Une pizza doit toujours avoir une pâte.");
                }
                if (pizzaVM.selectedIngredients.Count() < 2 || pizzaVM.selectedIngredients.Count() > 5)
                {
                    throw new Exception("Une pizza doit avoir entr e 2 et 5 ingrédients.");
                }

                if (pizzas.Any(p => p.Nom.ToUpper() == pizzaVM.Pizza.Nom.ToUpper() && pizzaVM.Pizza.Id != p.Id))
                {
                    ModelState.AddModelError("", "Il existe déjà une pizza portant ce nom.");
                    return View();
                }

                foreach (Pizza pizza in pizzas)
                {
                    if (pizzaVM.selectedIngredients.Count() == pizzaVM.Pizza.Ingredients.Count())
                    {
                        List<Ingredient> pizzaBd = pizzaVM.Pizza.Ingredients.OrderBy(x => x.Id).ToList();
                        pizzaVM.selectedIngredients = pizzaVM.selectedIngredients.OrderBy(x => x).ToList();
                        bool isDifferent = false;
                        for (int i = 0; i < pizzaVM.selectedIngredients.Count(); i++)
                        {
                            if (pizzaVM.selectedIngredients.ElementAt(i) != pizzaBd.ElementAt(i).Id)
                            {
                                isDifferent = true;
                                break;
                            }
                        }
                        if (!isDifferent)
                        {
                            throw new Exception("Il existe une pizza avec ces ingrédients.");
                        }
                    }
                }

                Pizza pizzaDb = pizzas.FirstOrDefault(p => p.Id == pizzaVM.Pizza.Id);
                pizzaDb.Nom = pizzaVM.Pizza.Nom;
                pizzaDb.Pate = listPates.FirstOrDefault(p => p.Id == pizzaVM.selectedPate);
                foreach (var ingredient in pizzaVM.selectedIngredients)
                {
                    pizzaDb.Ingredients.Add(listIngredients.FirstOrDefault(i => i.Id == ingredient));
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                pizzaVM.setIngredients(listIngredients);
                pizzaVM.setPates(listPates);
                if (pizzaVM.Pizza.Pate != null)
                {
                    pizzaVM.selectedPate = pizzaVM.Pizza.Pate.Id;
                }

                if (pizzaVM.Pizza.Ingredients.Any())
                {
                    pizzaVM.selectedIngredients = pizzaVM.Pizza.Ingredients.Select(i => i.Id).ToList();
                }

                return View(pizzaVM);
            }
        }

        // GET: Pizza/Delete/5
        public ActionResult Delete(int id)
        {
            Pizza pizza = pizzas.FirstOrDefault(p => p.Id == id);
            if (pizza != null)
            {
                return View(pizza);
            }
            return RedirectToAction("Index");
        }

        // POST: Pizza/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Pizza pizza = pizzas.FirstOrDefault(p => p.Id == id);
                pizzas.Remove(pizza);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}