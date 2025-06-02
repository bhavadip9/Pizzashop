
using Microsoft.EntityFrameworkCore;
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class SectionRepository : ISectionRepository
    {
        private readonly NewPizzashopContext _context;

        public SectionRepository(NewPizzashopContext context)
        {
            _context = context;

        }


        public async Task<List<Section>> GetAllSection()
        {
            try
            {
                return await _context.Sections
                       .Where(c => !c.IsDeleted).OrderByDescending(c=>c.SectionId).ToListAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }
        
        public List<Section> GetAllSectionList()
        {
            try
            {
                return _context.Sections
                       .Where(c => !c.IsDeleted).OrderByDescending(c=>c.SectionId).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;

            }
        }
        public List<Table> GetAllTableList(int sectionId)
        {
            try
            {
                var result = _context.Tables.Where(c => c.SectionId == sectionId).Where(c => !c.IsDelete).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }

        }


        public Section getSection(int id)
        {
            try
            {
                var section = _context.Sections.Find(id);
                return section!;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }


        }

        public Section AddSection(Section section)
        {
            try
            {
                _context.Sections.Add(section);
                _context.SaveChanges();
                return section;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }

        }

        public bool UpdateSection(Section section)
        {
            try
            {
                _context.Sections.Update(section);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }


        public bool DeleteSection(int id)
        {
            try
            {
                var category = _context.Sections
                    .Include(c => c.Tables)
                    .FirstOrDefault(c => c.SectionId == id);

                if (category == null)
                    return false;

                foreach (var item in category.Tables)
                {
                    item.IsDelete = true;
                }

                category.IsDeleted = true;
                _context.Sections.Update(category);

                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public AddTableViewModel AddTable(AddTableViewModel table)
        {
            try
            {
                var tabledata = new Table
                {
                    SectionId = table.SectionId,
                    TableName = table.TableName,
                    Capacity = table.Capacity,
                    Status = table.Status

                };

                _context.Tables.Add(tabledata);
                _context.SaveChangesAsync();
                return null!;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }

        }



        public async Task<List<Table>> GetAllTable(int sectionId)
        {
            try
            {
                var result = await _context.Tables.Where(c => c.SectionId == sectionId).Where(c => !c.IsDelete).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }
        public Table GetTable(int id)
        {
            return _context.Tables.FirstOrDefault(m => m.TableId == id)!;
        }

        public Table EditTable(Table table)
        {
            try
            {
                _context.Tables.Update(table);
                _context.SaveChanges();
                return table;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }

        }

        public void DeleteTable(int id)
        {
            var table = _context.Tables.Find(id);
            if (table == null)
            {
                return;
            }
            table.IsDelete = true;
            _context.Tables.Update(table);
            _context.SaveChanges();
        }

        public void DeletemanyTable(List<int> Ids)
        {
            var tableToDelete = _context.Tables.Where(item => Ids.Contains(item.TableId)).ToList();
            foreach (var table in tableToDelete)
            {
                table.IsDelete = true;
            }
            _context.Tables.UpdateRange(tableToDelete);
            _context.SaveChanges();
        }

    }


}