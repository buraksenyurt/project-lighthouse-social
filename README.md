# project-lighthouse-social

C# ile uçtan uca bir Web projesi geliştirilmesi. Konu, dünya üzerindeki deniz fenerlerine ait fotoğrafların paylaşıldığı, yorumlandığı ve puanlandığı bir sosyal platform. _(Proje ilerleyişi [Youtube](https://youtu.be/XUiG1MwSq1o?si=qQbWwclg3pPuYDaF) kanalından da takip edebilirsiniz)_ Projede mümkün mertebe yazılım dünyasının efsane konularına olan ihtiyaçları ortaya koymaya çalışmak ilk amaçlarımdan birisidir. Örneğin, hiçbir mimari kalıba uymadan sadece belli prensipleri _(soyutlamalar, bağımlılıkları tersine çevirme, sorumlulukları dağıtma vs gibi)_ baz alarak bir proje iskeleti oluşturup, sonrasında sorularla yaklaşımın doğruluğunu değerlendirmek, açık noktaları tespit etmek ve standartlaşmış bir mimari kalıba çevirmek gibi.

---

## Proje Konusu

Deniz Feneri meraklıları için bir sosyal paylaşım platformu geliştirmek.

## Projenin Genel Özellikleri

- Platform kullanıcıları çektikleri deniz feneri **fotoğraflarını** paylaşabilirler.
- Kullanıcılar dünyanın dört bir yanında yer alan deniz fenerleri hakkında **kapsamlı ve detaylı bilgileri** öğrenebilirler.
- Kullanıcılar deniz feneri fotoğraflarına **yorum** bırakabilir ve puanlayabilirler.

## Amaçlar

Bu projeyi geliştirmenin temel amaçları aşağıda listelenmiştir.

- C# ve .Net platformunu örnek bir proje geliştirerek tanımak.
- Düzenli olarak refactoring uygulayıp kodu iyileştirmeye çalışmak.
- AI Asistanlarından yararlanmak _(minimum ölçüde)_
- Temel yazılım prensiplerini keşfetmek, uygulamak ve sorgulamak.
- Yazılım mimarilerinin ihtiyaçlarını fark etmeye çalışıp, tartışmak, uygulamak.

## Zorluklar _(Challenges)_

- Kullanıcıların paylaştığı fotoğrafları nasıl ve nerede tutacağız? _(Boyut, depolama yeri, yazma/okuma hızları, dağıtık topoloji kullanımları)_
- Kullanıcı yorumlarının denetlenmesi ve istenmeyen ifadelerin engellenmesi nasıl sağlanır?
- Bir fotoğrafın doğru deniz fenerine ait olduğu nasıl tespit edilir?
- Fotoğraflardaki özgünlüğü anlamak için kategorilendirme veya tag'leme aşamasında AI araçlarından nasıl yararlanılır?
- Çok sayıda kullanıcının farklı lokasyonlardan fotoğraf yüklemesi halinde fotoğrafın analizi, doğrulanması, sınıflandırılması gibi hizmetlerin sistemin genelini etkilemeden en hızlı şekilde yapılması nasıl sağlanır?
- Çözüme dahil olan harici servislerin oluşturacağı dağıtık sistemde kaotik durumların önüne nasıl geçilir, sistemin dayanıklılığı nasıl sağlanır?

## İçerik Planı

- [x] **Bölüm 00 Proje Tanıtımı ve Solution Açılması:** Projenin amacı anlatılır, teknoloji seçimine değinilir ve Solution açılır.
  - [Video 00](https://youtu.be/xO4S60bfZPU)
- [x] **Bölüm 01 Domain Model Tasarımı:** User, Lighthouse, Photo, Comment vb temel sınıflar belirlenir.
  - [Video 01](https://youtu.be/fIsvAwxnnIQ)
  - [Video 02](https://youtu.be/dDZHq-vI18U)
  - [Video 03](https://youtu.be/cCW44l7fgX0)
  - [Video 04](https://youtu.be/_cta_s9zM1U)
- [ ] **Bölüm 02 Application Katmanı, CRUD Sözleşmelerinin İnşaası... :** Repository/Service katmanları için interface tasarımları ve application katmanının yazılması.
  - [Video 05](https://youtu.be/SmnrE73VvUo)
  - [Video 06](https://youtu.be/x6u7uMxw8qU)
  - [Video 07](https://youtu.be/P_uRenWyE54)
  - [Video 08](https://youtu.be/MrqTqc9d2q8)
- [x] **Ara Bölüm, Sonarqube:** Kod tabanı büyüdükçe ortaya çıkabilecek teknik borçları önlemek için baştan tedbir alıyoruz.[Video 09](https://youtu.be/XUiG1MwSq1o)
  - [Video 10](https://youtu.be/54GMi9i2W-4)

## Sonarqube

Teknik borçlanmanın önüne geçmek için statik kod analiz aracı olarak **Sonarqube** kullanılmaktadır. Local ortamda **docker-compose** ile ayağa kaldırılan üründe tarama başlatmak için aşağıdaki hazırlıkları yapmak yeterlidir.

```bash
# Dotnet için gerekli tarama aracının yüklenmesi
dotnet tool install --global dotnet-sonarscanner

# Solution klasöründe ise aşağıdaki komutların çalıştırılması yeterlidir.
# Elbette token bilgisi sizin kurulumunuza göre değişiklik gösterecektir.
dotnet sonarscanner begin /k:"Project-Ligthouse-Social" /d:sonar.host.url="http://localhost:9000"  /d:sonar.token="sizin_için_üretilen_token"

dotnet build

dotnet sonarscanner end /d:sonar.token="sizin_için_üretilen_token"
```

Belirli periyotlarda Sonarqube taraması yaparak projenin kod kalite uyumluluğu ölçümlenebilir. Ölçmekte fayda vardır.

## JudgeDredd Servisi için Dockerize İşlemleri

```bash
# docker-compose dosyasında JudgeDredd klasörü için servis bildirimi yapılır.
# Sonrasında docker-compose dosyasının olduğu klasörde build işlemi başlatılır.
docker-compose build

# Ardından hizmetler ayağa kaldırılır.
docker compose up -d

# Testler için 5005 portundan hizmet veren JudjeDredd servisine talepler gönderilebilir
```

---

## Görevler, Sorular

> NOT ALDIĞIMIZ GÖREVLER VE SORULAR EKLENECEK

## Kontrol Listesi

- [ ] C# dilinin temel özelliklerine yer verildi mi?
- [ ] OOP prensipleri uygulandı mı?
- [ ] SOLID prensiplerine yer verildi mi?
- [ ] Kod bazlı teknik borçlardan arındırıldı mı?
- [ ] Belli bir yazılım mimari stiline evrildi mi?
- [ ] Projede en az bir Rest tabanlı Web Api kullanıldı mı?
- [ ] Projede gRPC tabanlı bir servis kullanıldı mı?
- [ ] Razor tabanlı Web uygulaması geliştirildi mi?
- [ ] Farklı dillerde yazılmış servisler kullanıldı mı?
- [ ] Bir dağıtık sistem kurgusu tesis edildi mi?
- [ ] Dağıtık sistem kurgusu tesis edildiyse relaibility için gerekli tedbirler alındı mı?
- [ ] Dağıtık sistem kurugusu için izleme, loglama, alarm mekanizmaları vs kullanıldı mı?
