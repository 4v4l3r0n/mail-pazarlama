# Mail Pazarlama Uygulaması

Kullanıcıların kayıtlı abonelere duyuru, kampanya, reklam gibi mailler gönderebildiği ve gönderimlerin sonucunu görebildiği bir mail uygulaması.

## Uygulama ne yapıyor?

İki bölümden oluşuyor:

1. **Herkese açık sayfa:** Ziyaretçi mail adresini yazıp "Kaydet" diyerek abone oluyor.
2. **Yönetim paneli:** Giriş yapan kullanıcı aboneleri yönetiyor, mail şablonları hazırlıyor, seçtiği abonelere şablonu gönderiyor ve gönderim sonuçlarını raporlardan takip ediyor.

```
Ziyaretçi abone olur -> Kullanıcı şablon hazırlar -> Abone ve şablon seçip gönderir
                                                              |
                      Raporlama sayfasından sonuçlar görülür <-
```

## Kullanılan teknolojiler

| Ne için | Ne kullanılıyor |
|---|---|
| Dil ve platform | C# ve .NET 8 |
| Web arayüzü | ASP.NET Core MVC + Razor |
| Veritabanı | MS SQL Server |
| Veritabanı erişimi | Entity Framework Core |
| Mail gönderme | MailKit (SMTP) |
| Arka plan işleri | `BackgroundService` + kuyruk |
| HTML editör | Hazır WYSIWYG editör (TinyMCE gibi) |
| Tasarım | Hazır Bootstrap tabanlı HTML tema |
| Test için SMTP | smtp4dev |
| Versiyon takibi | Git ve GitHub |

## Mimari: 4 katman

```
Web        sayfalar, formlar, butonlar
  |
Business   kurallar: "mail formatı geçerli mi?", "bu kayıt silinebilir mi?"
  |
Data       veritabanıyla konuşan kısım
  |
Entities   tablo karşılığı sınıflar: User, Subscriber...
```

| Katman | İçerik |
|---|---|
| **Entities** | Sadece sınıflar: `User`, `Subscriber`, `MailTemplate`, `MailSetting`, `MailSendLog` |
| **Data** | `DbContext`, tablo ayarları, veritabanını okuyan ve yazan sınıflar (repository), migration'lar |
| **Business** | Kullanıcı, abone, şablon, gönderim, rapor ve ayar servisleri; doğrulamalar; parola şifreleme; mail gönderimi |
| **Web** | Controller'lar, Razor sayfaları (View), giriş ve oturum kontrolü |

```
source_code/
  MailPazarlama.sln
  MailPazarlama.Entities/
  MailPazarlama.Data/
  MailPazarlama.Business/
  MailPazarlama.Web/
```

## Veritabanı

5 tablo olacak. Kalın yazılan alanlar başka bir tabloya bağlanan (Foreign Key) alanlar.

| Tablo | İçerik | Alanlar |
|---|---|---|
| **Users** | Panel kullanıcıları | Id, Ad, Soyad, Email (tekil), Şifreli parola, Aktif mi, Kayıt zamanı |
| **Subscribers** | Aboneler | Id, Email (tekil), Kayıt zamanı |
| **MailTemplates** | Mail şablonları | Id, Başlık, Html içerik, Aktif mi, Kayıt zamanı, **Kaydeden kullanıcı Id** |
| **MailSettings** | SMTP ayarı | Id, Sunucu adresi, Port, SSL açık mı, Mail adresi, Şifreli parola |
| **MailSendLogs** | Her alıcı için bir gönderim kaydı | Id, **Abone Id**, **Şablon Id**, **Gönderen kullanıcı Id**, Gönderim zamanı, Durum (Başarılı/Başarısız), Açıklama (hata mesajı) |

İlişkiler:
- Bir kullanıcı birçok şablon kaydedebilir.
- Bir abone ve bir şablon birçok gönderim kaydında yer alabilir.
- Bir kullanıcı birçok gönderim yapabilir.

## Sayfalar ve özellikler

### Herkese açık

**Abone olma sayfası**
- Uygulama adı, görsel ve kısa açıklama
- Mail alanı ve Kaydet butonu
- Mail formatı kontrolü, hatalıysa uyarı
- Aynı mail zaten kayıtlıysa uyarı
- Başarılı kayıtta bilgi mesajı

### Giriş öncesi

**Kullanıcı kayıt**
- Mail, parola, parola tekrar, ad-soyad
- Boş alan ve mail formatı kontrolü
- Parola en az 8 karakter olmalı, büyük harf, küçük harf ve rakam içermeli
- Aynı mail ile ikinci kayıt yapılamıyor
- Başarılı kayıtta bilgi mesajı, parola şifreli saklanıyor

**Giriş**
- Mail ve parola alanları, parola `*` olarak görünüyor
- Boş alan uyarısı
- Kayıt ve parola hatırlatma sayfalarına link
- Kullanıcı yoksa ve parola yanlışsa farklı mesajlar
- Başarılı girişte ana sayfaya yönlendirme

**Parola hatırlatma**
- Sayfa açılınca sadece mail alanı ve Doğrula butonu var
- Mail kayıtlıysa parola ve parola tekrar alanları açılıyor, değilse "kullanıcı bulunamadı" mesajı çıkıyor
- Yeni parola kayıttaki kurallara uymalı

### Panel (giriş gerekli)

**Ana sayfa**
- Kullanıcı adı belirgin şekilde görünüyor, güvenli çıkış butonu var
- Abone, şablon ve gönderim sayıları
- Profil, Abone, Şablon, Gönderim, Raporlama ve Tanım sayfalarına linkler

**Profil**
- Mail, parola ve ad-soyad güncellenebiliyor
- Kayıt sayfasındaki tüm kontroller burada da geçerli; başka bir kullanıcının maili girilemiyor

**Abone**
- Liste: mail ve kayıt zamanı; mail ve kayıt zamanına göre arama
- Yeni abone ekleme
- Silme: "Kaydı silmek istediğinize emin misiniz?" onayı, gönderim yapıldıysa silinemiyor

**Şablon**
- Liste: başlık, kayıt zamanı, kaydeden kullanıcı, aktiflik durumu
- Başlık ve HTML editörle yeni şablon oluşturma, boş bırakılırsa uyarı
- Şablonu pasife alma
- Silme: onay istiyor, gönderim yapıldıysa silinemiyor

**Gönderim**
- Abone listesi (sadece mail ve seçim kutusu), bir ya da birden çok abone seçilebiliyor
- Aktif şablonların açılır menüsü, seçmek zorunlu
- "Gönderim yapmak istediğinize emin misiniz?" onayı
- Asenkron gönderim, zaman aşımı, her alıcı için durum ve hata mesajı kaydı

**Raporlama**
- Şablon açılır menüsü
- Liste: mail, gönderim zamanı, durum, açıklama
- Şablona, tarih aralığına ve gönderim durumuna göre filtreleme

**Tanım (SMTP ayarları)**
- Mail sunucusu, port, SSL onay kutusu, mail adresi ve parola
- Kaydederken tüm alanlar dolu olmalı, boşsa uyarı veriyor

**Kullanıcı**
- Liste: ad, soyad, mail, aktiflik durumu, kayıt zamanı
- Seçilen kullanıcıyı pasife alma
