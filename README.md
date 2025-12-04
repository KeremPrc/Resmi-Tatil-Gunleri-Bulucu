📌 PublicHolidayTracker – C# Console Application

Bu proje, Türkiye’deki resmi tatil bilgilerini Nager.Date Public Holiday API üzerinden alarak kullanıcıya çeşitli sorgulama seçenekleri sunan bir C# Konsol Uygulamasıdır.
Uygulama, 2023–2025 yıllarına ait tatil verilerini API’den çeker, hafızaya alır ve kullanıcı etkileşimli bir menü aracılığıyla çeşitli sorgulamalar yapabilir.

----------------------------------------------

🚀 Özellikler:

- Yıllık resmi tatil listesini API üzerinden çekme

- JSON → C# sınıf dönüşümü

- Verileri hafızada saklama (cache)

- Tarih ile tatil arama (gg-aa formatı)

- İsim ile tatil arama

- 3 yıl (2023–2025) tüm tatilleri listeleme

- Tatil listesini tarihe göre sıralama

- Tam etkileşimli konsol menüsü
  
----------------------------------------------

🛠 Kullanılan Teknolojiler

- C# .NET 6 / .NET 7

- HttpClient (API tüketimi)

- System.Text.Json (JSON dönüşümü)

- VS Code veya Visual Studio

----------------------------------------------

📡 Kullanılan API

Uygulama şu adreslerden veri çeker:

https://date.nager.at/api/v3/PublicHolidays/2023/TR
https://date.nager.at/api/v3/PublicHolidays/2024/TR
https://date.nager.at/api/v3/PublicHolidays/2025/TR
