# 🥗 Diyetisyen-Hasta Takip Uygulaması

Bu proje, diyetisyenlerin hastalarıyla iletişim kurmasını, randevu ve diyet planlarını yönetmesini, hastaların ise bu planlara erişmesini sağlayan web tabanlı bir uygulamadır.

## 🎯 Amaç

- Diyetisyen ve hasta arasındaki iletişimi dijital ortamda kolaylaştırmak
- Diyetisyenlerin bireysel hastalarına özel programlar oluşturabilmesini sağlamak
- Hastaların güncel diyet planlarını ve hedeflerini takip edebilmesini mümkün kılmak

## 🚀 Özellikler

- Kullanıcı kimlik doğrulama (JWT + Role-based Authorization)
- Diyetisyen ve hasta rolleri için özelleştirilmiş arayüzler
- Randevu planlama, iptal etme ve geçmişi görüntüleme
- Gerçek zamanlı bildirim sistemi (SignalR)
- Diyet planı ekleme, düzenleme ve hasta tarafında görüntüleme
- Raporlama ve geçmişe dönük veri inceleme

## 🛠 Kullanılan Teknolojiler

- ASP.NET Core Web API
- Entity Framework Core
- Onion Architecture
- SOLID Principles
- CQRS & Mediator Pattern
- MongoDB, Redis
- JWT Authentication
- SignalR (Gerçek zamanlı iletişim)
- RabbitMQ (ESB)
- Git & GitHub (versiyon kontrolü ve takım çalışması)
- Agile Scrum + Jira (proje yönetimi)

## 📦 Projeyi Çalıştırmak

1. Bu repository'i klonlayın:
   ```bash
   git clone https://github.com/ibrahimkahramann/Dotnet-Dietitian
   ```

2. Gerekli bağlantı stringlerini ve app ayarlarını `appsettings.Development.json` dosyasına ekleyin.

3. Gerekli NuGet paketlerini yükleyin:
   ```bash
   dotnet restore
   ```

4. Veritabanını migrate edin:
   ```bash
   dotnet ef database update
   ```

5. Uygulamayı çalıştırın:
   ```bash
   dotnet run
   ```

## 📅 Sprint Planı

- Sprint 1: Temel altyapının kurulumu, kullanıcı yönetimi, rol bazlı yetkilendirme
- Sprint 2: Diyet planı yönetimi, randevu sistemi
- Sprint 3: Bildirimler, gerçek zamanlı özellikler, gelişmiş kullanıcı arayüzleri
- Devam eden sprintlerle birlikte proje genişletilecek.

## 👥 Katkıda Bulunanlar

- [@ibrahim](https://www.linkedin.com/in/ibrahim-kahraman) (Team Lead, Backend & DevOps)
- [@Suna](https://www.linkedin.com/in/suna-s/)
- [@Eren](https://www.linkedin.com/in/erenalikoca/)
- [@Mücahit](https://www.linkedin.com/in/m%C3%BCcahit-top%C3%A7uo%C4%9Flu-33a449266/)
- [@Can](https://www.linkedin.com/in/can-onay-96808529b/)

## 📄 Lisans

MIT License © 2025


> Bu proje bir eğitim çalışması kapsamında geliştirilmiştir ve ileri seviye yazılım mimarisi, takım çalışması ve proje yönetimi alanlarında deneyim kazandırmayı amaçlamaktadır.
