<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

List<WebRequestActivity> q = (from w in WebRequestActivities
							where !w.UserAgent.Contains("monitis")
							orderby w.DateCreated descending
							select w).ToList();
q.Dump();