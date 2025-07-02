
# 🎓 EduSphere - Eğitim Yönetim Platformu

## 📝 Proje Hakkında

**EduSphere**, öğrenciler ve eğitimciler için geliştirilmiş modern, kullanıcı dostu bir eğitim yönetim platformudur. ASP.NET Core ve MongoDB teknolojileri kullanılarak geliştirilmiş olan bu platform, akademik hayatı kolaylaştırmak ve eğitim süreçlerini dijitalleştirmek amacıyla tasarlanmıştır.

![Platform Görünümü](https://img.shields.io/badge/Platform-ASP.NET%20Core-blue)
![Veritabanı](https://img.shields.io/badge/Database-MongoDB-green)
![Framework](https://img.shields.io/badge/Framework-.NET%208.0-purple)
![Frontend](https://img.shields.io/badge/Frontend-HTML%2FCSS%2FJS-orange)

## 🚀 Temel Özellikler

### 👨‍🎓 Öğrenci Özellikleri
- **Ders Programı Takibi**: Bölümlere göre organize edilmiş ders programları
- **Sınav Takvimi**: Kişiselleştirilmiş sınav tarih ve saatleri
- **Sınav Sonuçları**: Geçmiş sınav notları ve başarı durumu takibi
- **Duyuru Sistemi**: Anlık okul duyuruları ve bilgilendirmeler
- **Profil Yönetimi**: Kişisel bilgi güncelleme ve profil fotoğrafı yönetimi
- **Mesajlaşma Sistemi**: Güvenli medya paylaşımlı mesajlaşma
- **Sosyal Timeline**: Deneyim paylaşımı ve etkileşim

### 👨‍💼 Admin Özellikleri
- **Kullanıcı Yönetimi**: Öğrenci ve personel hesap yönetimi
- **Ders Programı Yönetimi**: Bölüm bazlı ders programı oluşturma/düzenleme
- **Sınav Yönetimi**: Sınav oluşturma, düzenleme ve sonuç girişi
- **Duyuru Yönetimi**: Genel duyuru ve bilgilendirme sistemi
- **Etkinlik Yönetimi**: Okul etkinlikleri ve organizasyon takibi
- **İstatistikler**: Kapsamlı raporlama ve analiz araçları

## 🛠️ Teknoloji Stack

### Backend
- **ASP.NET Core 8.0** - Web framework
- **MongoDB** - NoSQL veritabanı
- **Entity Framework Core** - ORM
- **SignalR** - Gerçek zamanlı iletişim

### Frontend
- **HTML5/CSS3** - Modern web standartları
- **JavaScript** - İnteraktif kullanıcı deneyimi
- **Bootstrap** - Responsive tasarım
- **Font Awesome** - İkon kütüphanesi

### Veritabanı Yapısı
```
Collections:
├── Users          # Kullanıcı bilgileri
├── Schedules      # Ders programları
├── Exams          # Sınav bilgileri
├── ExamResults    # Sınav sonuçları
├── Posts          # Sosyal paylaşımlar
├── Messages       # Mesajlaşma
└── Events         # Etkinlikler
```

## 📁 Proje Yapısı

```
EduSphere/
├── Controllers/           # MVC Controller'ları
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── AcademyController.cs
│   ├── DashboardController.cs
│   ├── TimelineController.cs
│   └── MessagesController.cs
├── Models/               # Veri modelleri
│   ├── User.cs
│   ├── Schedule.cs
│   ├── Exam.cs
│   ├── ExamResult.cs
│   ├── Post.cs
│   └── Message.cs
├── Views/                # Razor view dosyaları
│   ├── Home/
│   ├── Account/
│   ├── Admin/
│   ├── Academy/
│   └── Shared/
├── wwwroot/             # Statik dosyalar
│   ├── css/
│   ├── js/
│   └── images/
└── Data/                # Veritabanı context
```

## 🎯 Ana Modüller

### 1. Kullanıcı Yönetimi
- Öğrenci ve admin rolleri
- Güvenli kimlik doğrulama
- Profil yönetimi ve fotoğraf desteği
- Bölüm bazlı organizasyon

### 2. Akademik Yönetim
- **Ders Programları**: Günlük/haftalık ders takvimleri
- **Sınav Takvimi**: Vize, final ve bütünleme sınavları
- **Not Sistemi**: AA-FF arası harf notları
- **Bölüm Sistemi**: 6 farklı mühendislik bölümü desteği

### 3. İletişim Sistemi
- Güvenli mesajlaşma
- Medya dosyası paylaşımı (resim/video)
- Gerçek zamanlı bildirimler
- Sosyal timeline

### 4. Etkinlik Yönetimi
- Okul etkinlikleri
- Konum bilgisi ile etkinlik takibi
- Katılım yönetimi

## 🏗️ Kurulum ve Çalıştırma

### Gereksinimler
- .NET 8.0 SDK
- MongoDB Community Server
- Visual Studio 2022 veya VS Code

### Kurulum Adımları

1. **Projeyi klonlayın**
```bash
git clone https://github.com/oxygeneafk/EduSphere.git
cd EduSphere
```

2. **MongoDB Bağlantısını Yapılandırın**
`appsettings.json` dosyasında MongoDB connection string'ini güncelleyin:
```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017/EduSphere"
  }
}
```

3. **Bağımlılıkları Yükleyin**
```bash
dotnet restore
```

4. **Projeyi Çalıştırın**
```bash
dotnet run
```

5. **Tarayıcıda Açın**
Uygulama `https://localhost:7000` adresinde çalışacaktır.

### Varsayılan Admin Hesabı
```
Kullanıcı Adı: admin
Şifre: 123
```

## 🎨 Özellik Detayları

### Responsive Tasarım
- Mobil, tablet ve masaüstü uyumlu
- Modern gradient ve animasyonlar
- Kullanıcı dostu arayüz
- Dark theme desteği

### Güvenlik
- Form doğrulama ve CSRF koruması
- Güvenli şifre yönetimi
- Role-based authorization
- Güvenli dosya yükleme

### Performans
- MongoDB optimizasyonları
- Asenkron işlemler
- Sayfalama ve filtreleme
- Önbellekleme mekanizmaları

## 🔮 Gelecek Geliştirmeler

- [ ] E-posta bildirimleri
- [ ] Push notification desteği
- [ ] API geliştirme
- [ ] Mobil uygulama
- [ ] Gelişmiş raporlama
- [ ] Çoklu dil desteği
- [ ] Video konferans entegrasyonu
- [ ] Ödev sistemi

## 🤝 Katkıda Bulunma

Bu proje aktif olarak geliştirilmektedir. Katkıda bulunmak için:

1. Projeyi fork edin
2. Feature branch oluşturun (`git checkout -b feature/YeniOzellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/YeniOzellik`)
5. Pull Request oluşturun

## 👥 Desteklenen Bölümler

- Computer Engineering
- Computer Programming  
- Electricity Electronic Engineering
- Machinery Engineering
- Civil Engineering
- Industrial Engineering

## 📞 İletişim

**Geliştirici**: [@oxygeneafk](https://github.com/oxygeneafk)

**Proje Linki**: [https://github.com/oxygeneafk/EduSphere](https://github.com/oxygeneafk/EduSphere)

## 📄 Lisans

Bu proje açık kaynak kodlu olarak geliştirilmiştir.

---

### 🌟 EduSphere ile Eğitimin Geleceğini Keşfedin!

*Bu platform, modern eğitim ihtiyaçlarını karşılamak ve akademik süreçleri dijitalleştirmek amacıyla geliştirilmiştir.*
