<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

Countries.DeleteAllOnSubmit(Countries);
SubmitChanges();

Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "ago", CountryName = "Angola", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "aus", CountryName = "Australia", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "aut", CountryName = "Austria", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "bel", CountryName = "Belgium", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "bih", CountryName = "Bosnia and Herzegovina", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "bra", CountryName = "Brazil", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "can", CountryName = "Canada", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "chn", CountryName = "China", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "col", CountryName = "Colombia", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "cze", CountryName = "Czech Republic", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "deu", CountryName = "Germany", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "dnk", CountryName = "Denmark", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "est", CountryName = "Estonia", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "fin", CountryName = "Finland", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "gbr", CountryName = "United Kingdom", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "grc", CountryName = "Greece", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "hkg", CountryName = "Hong Kong", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "hrv", CountryName = "Croatia", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "hun", CountryName = "Hungary", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "imn", CountryName = "Isle of Man", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "irl", CountryName = "Ireland", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "isl", CountryName = "Ice Land", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "isr", CountryName = "Israel", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "ita", CountryName = "Italy", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "jpn", CountryName = "Japan", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "kor", CountryName = "Korea (South)", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "ltu", CountryName = "Lithuania", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "lux", CountryName = "Luxembourg", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "lva", CountryName = "Latvia", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "mex", CountryName = "Mex", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "mkd", CountryName = "Macedonia", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "nld", CountryName = "Netherlands", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "nor", CountryName = "Norway", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "nzl", CountryName = "New Zealand", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "pol", CountryName = "Poland", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "prt", CountryName = "Portugal", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "rou", CountryName = "Romania", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "rus", CountryName = "Russian Federation", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "sgp", CountryName = "Singapore", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "srb", CountryName = "Serbia", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "svk", CountryName = "Slovakia", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "svn", CountryName = "Slovenia", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "swe", CountryName = "Sweden", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "ukr", CountryName = "Ukraine", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "usa", CountryName = "United States of America", DateCreated = DateTime.Now});
Countries.InsertOnSubmit(new Country(){CountryId = Guid.NewGuid(), CountryCode = "zaf", CountryName = "South Africa", DateCreated = DateTime.Now});

SubmitChanges();
Countries.OrderBy (c => c.CountryName).Dump();