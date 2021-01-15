<Query Kind="Expression">
  <Connection>
    <ID>43a4e350-a857-4c1a-a516-57605953ef5d</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="NY.CommonDataService.LINQPadDriver" PublicKeyToken="no-strong-name">NY.CommonDataService.LINQPadDriver.DynamicDriver</Driver>
    <DisplayName>DreaminginCRM</DisplayName>
    <DriverData>
      <EnvironmentUrl>https://dreamingincrm.crm6.dynamics.com</EnvironmentUrl>
      <ApplicationId>9e15cf84-840a-416a-a612-a87de2915547</ApplicationId>
      <ClientSecret>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAt5GqVpdjVkGH7StDGkHhawAAAAACAAAAAAAQZgAAAAEAACAAAADgHj2rEf02rcpYok9NPzagekHGIHU2CCqXMD/CiySO0QAAAAAOgAAAAAIAACAAAABoLBhm4ievLMToFkDYEU5rALuadi9e5XGs8cbbk3BJ8DAAAADvfO5wOChEcEqVQrXKyGovr1jDhzaZl53dzzdLG11riMSzAPfBPtK9aRRt0qC1dulAAAAAlGZOXwW8auADA3N9xUx1s2RS76OfjiCMrJI0D7mL8zGf7m30JCfbP0JK7xMZgYttjtpNKfUPdvN7TyaikAJMww==</ClientSecret>
    </DriverData>
  </Connection>
</Query>

(from w in WebResource
where w.WebResourceType == WebResource_WebResourceType.JPG_format ||
w.WebResourceType == WebResource_WebResourceType.ICO_format ||
w.WebResourceType == WebResource_WebResourceType.GIF_format ||
w.WebResourceType == WebResource_WebResourceType.PNG_format ||
w.WebResourceType == WebResource_WebResourceType.Vector_format_SVG
select new {
w.Name,
w.DisplayName,
Image = w.WebResourceType == WebResource_WebResourceType.Vector_format_SVG ? Util.RawHtml(Encoding.ASCII.GetString(Convert.FromBase64String(w.Content))) : Util.Image(Convert.FromBase64String(w.Content))
}).Take(100)