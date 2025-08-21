# project-lighthouse-social

C# ile uçtan uca bir Web projesi geliştirilmesi. Konu, dünya üzerindeki deniz fenerlerine ait fotoğrafların paylaşıldığı, yorumlandığı ve puanlandığı bir sosyal platform. _(Proje ilerleyişi [Youtube](https://www.youtube.com/playlist?list=PLY-17mI_rla6Kt-Ri6nP1pE62ZyE-6wjS) kanalından da takip edebilirsiniz)_ Projede mümkün mertebe yazılım dünyasının efsane konularına olan ihtiyaçları ortaya koymaya çalışmak ilk amaçlarımdan birisidir. Örneğin, hiçbir mimari kalıba uymadan sadece belli prensipleri _(soyutlamalar, bağımlılıkları tersine çevirme, sorumlulukları dağıtma vs gibi)_ baz alarak bir proje iskeleti oluşturup, sonrasında sorularla yaklaşımın doğruluğunu değerlendirmek, açık noktaları tespit etmek ve standartlaşmış bir mimari kalıba çevirmek gibi.

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

## Bu Çalışma Kimlere Göre

Minimum Profil;

- Temel seviyede C# ile programlama bilgisine sahiptir.
- Temel seviyede Nesne Yönelimli Programlama _(Object Oriented Programming)_ bilgisi vardır.
- Temiz kod kavramı ve standartları hakkında farkındalık sahibidir.
- Doğrudan uygulamak yerine, sorgular, araştırır, ikna olur ve sonra uygular.

İdeal Profil;

- SOLID prensiplerini sorgular.
- Teknik Borç ile mücadele yöntemleri hakkında fikir sahibidir.
- Yazılım mimarilerine meraklıdır.
- Kendi sistemlerinde docker kullanır.
- Web Api dışında farklı servis geliştirme standartları olduğunu bilir.
- Dağıtık sistemlerin zorluklarına aşinadır.
- Doğrudan uygulamak yerine, sorgular, araştırır, ikna olur ve sonra uygular.

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
- [x] **Bölüm 02 Application Katmanı, CRUD Sözleşmelerinin İnşaası... :** Repository/Service katmanları için interface tasarımları ve application katmanının yazılması.
  - [Video 05](https://youtu.be/SmnrE73VvUo)
  - [Video 06](https://youtu.be/x6u7uMxw8qU)
  - [Video 07](https://youtu.be/P_uRenWyE54)
  - [Video 08](https://youtu.be/MrqTqc9d2q8)
- [x] **Ara Bölüm, Sonarqube:** Kod tabanı büyüdükçe ortaya çıkabilecek teknik borçları önlemek için baştan tedbir alıyoruz.[Video 09](https://youtu.be/XUiG1MwSq1o)
  - [Video 10](https://youtu.be/54GMi9i2W-4)
  - [Video 11](https://youtu.be/PU9SqkPt41o)
  - [Video 12](https://youtu.be/vdFRhsLcqhM)
- [x] **Bölüm 03 Persistance katmanının inşaası:** Postgresql tabanlı veritabanı hazırlıkları, Concrete Repository sınıflarınının oluşturulması.
  - [Video 13](https://youtu.be/z8G9iThiiDE)
  - [Video 14](https://youtu.be/pA8xsOmsZpI)
- [x] **Bölüm 04 Basit İstemci Uygulamasının Geliştirilmesi:** Application katmanını kullanan basit bir terminal uygulamasının geliştirilmesi ve başarılı şekilde çalışır hale getirilmesi.
  - [Video 15](https://youtu.be/ovhQM9L_hhQ)
  - [Video 16](https://youtu.be/Frbquqiq4Us)
- [x] **Bölüm 05 Infrastructure Katmanı Geliştirmeleri:** Fotoğraf saklama, doğrulama, caching, bildirim gönderimi vb altyapı bileşenlerinin geliştirilmesi.
  - [Video 17](https://youtu.be/RnCqWo9Bhs8)
  - [Video 18](https://youtu.be/pfPqZ1SkHdM) 
  - [Video 19](https://youtu.be/CN52vnOzfT4)
  - [Video 20](https://youtu.be/GadLQoJNfw4)
  - [Video 21](https://youtu.be/B8VHWE8xbZU)
  - [Video 22](https://youtu.be/24CHSgqMaB4)
  - [Video 23](https://youtu.be/z9t0rKR5g_8)
  - [Video 24](https://youtu.be/rrRxhuo8w0E)
  - [Video 25](https://youtu.be/gRPlFqGVTCQ)
- [x] **Bölüm 05 Application Katmanı Pipeline Düzenlemeleri:** Özellikle pipeline behaviors tasarımı, ortak handler sözleşmesi ve servis bileşeni düzenlemelerini ele alıyoruz.
  - [Video 26](https://youtu.be/rliAGCQpGmE)
  - [Video 27](https://youtu.be/Y1uZuewmLAs)
  - [Video 28](https://youtu.be/p1IQqc5cD6g)
- [ ] **Bölüm 06 Presentation Katmanı Geliştirmeleri:** Rest tabanlı Web Api ve OData servisleri, Razor Web App, Blazor gibi farklı istemci uygulamalarının geliştirilmesi.
  - [Video 29](https://youtu.be/WI9lOd8uVs4)
  - [Video 30](https://youtu.be/EO1IED3soQ8)
  - [Video 31](https://youtu.be/8rV-mUXj4YQ)

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

- [ ] **Yorum Denetiminde Caching:** **JudjeDredd** servisi bir metin içeriğini denetlemek için **OpenAI Moderation API**'sine gidiyor. Saldırı türünden aynı yorumun defalarca gönderildiği bir durumda, **OpenAI** servisine sayısız kez gidilebilir. Gelen isteklerden aynı olanlar için daha önceden alınmış cevaplar _(Örneğin zaten flagged=true olanları)_ belli süreliğini cache'te tutulup anında cevap dönülebilir.
  - [ ] Aynı isteğin birden fazla defa gönderilmesi bir saldırı işareti de olabilir. Bunu tespit edip tedbir alan ve uyarı veren bir düzenek de geliştirilebilir.
- [ ] **Repository Sözleşmeleri:** Repository sözleşmelerinin **Domain** katmanında durması doğru mu? Ya da domain katmanında duracaklarsa hangi davranışları içeren sözleşmeler konulmalı? Sadece **Create, Retrieve, Update, Delete** fonksiyonelliklerini taşıyan ve Domain'i doğrudan ilgilendiren sözleşmeler buraya konup **Business Rules** içeren sözleşmeler farklı bir yere mi alınmalıdır?
- [ ] **Handler Bileşenlerine Erişim:** Projenin çekirdek katmanı **Appliacation** kütüphanesi. Bu katman içinden UI, Api, Terminal gibi istemcilere açtığımız sözleşmeler _(Contracts klasörü)_ düşünüldüğünde **Handler** sınıfları dışarıya kapatılmalı mıdır?
- [ ] **Unit of Work:** Handler bileşenlerindeki **HandleAsync** metotları belli bir akışa sahip kodları işletmekte. **DTO** doğrulamaları, farklı iş kuralları, transaction'a dahil işlemler, loglama vs Bu akış deseni bir üst yapıda mı toplanmalı? _(Bir unit of work içerisinde mesela)_
- [ ] **Photo Storage:** Projede fiziksel alan olarak en çok yer kaplayacak ve en çabuk büyüyecek kısım deniz feneri fotoğrafları. Fotoğrafların depolanacağı yer olarak local bir container kullanılabilir mi? Örneğin **AWS S3 Api** uyumlu [MinIO](https://min.io/docs/minio/container/index.html) veya muadili başka bir depo.
- [ ] **Persistence:** Proje domaininde yer alan Entity'ler sayıca çok veya içerik olarak karmaşık değiller. Veriler şema bazlı bir veritabanı sisteminde tutulabilir _(Örn: **Postgresql**)_. İlk etapta **Entity Framework Core** ve **Code-First** yerine **Database First** modelde ilerlenebilir ve erişimler için **Dapper**'dan yararlanılabilir.
- [ ] **Service Health Check/Discovery:** **JudgeDredd** gibi harici servislerin sayısı artacak. Ayrıca sistem büyüdükçe **Postgresql Server**, **Storage Service**, **RabbitMQ**, **Keycloak** vb birçok enstrüman da çözüme dahil edilecek. Bu servislerin ayakta olup olmadıklarının kontrolü ve hatta ortamlara göre değişecek port veya ip bilgilerinin kolayca yönetimi gerekecek. Bu amaçla [HashiCorp'un Consul](https://developer.hashicorp.com/consul/docs/intro) aracından yararlanılabilir.
- [ ] **Membership Management:** Sisteme dahil olacak abonelerin doğrulama _(Authentication)_ işlemleri için hibrit bir model tercih edilebilir. **Identity Provider** olarak **[Keycloak](https://www.keycloak.org/)** kullanılabilir, kullanıcı profil bilgileri veritabanında saklanabilir. Projemiz **authentication** detayları ile uğraşmak zorunda kalmaz.
- [ ] **Secret Keys:** Projede veritabanı adresi, api key, api secret gibi şifrelenmesi ve ele geçirilmemesi gereken bilgiler yer alacak. Bunların daha güvenli bir ortamda tutulup çalışma zamanında çözümlenerek kullanılması yerinde olacaktır. Gizli değerlerin yönetimi içim bir Vault sisteminden yararlanılabilir. **[Hashicorp Vault](https://developer.hashicorp.com/vault)** veya **[Localstack](https://github.com/localstack/localstack)** olabilir.
- [ ] **CLI Aracı:** CLI _(Command Line Interface)_ kullanma bilgisi olanlar son kullanıcılar için bir terminal aracı geliştirilebilir mi?
- [ ] **Public API:** Projenin genel kullanıma açık bir API hizmeti olabilir mi? Örneğin, deniz feneri bilgilerini dış dünyaya açabiliriz. Bu, standart web arayüzü dışında bir hizmettir, farklı uygulamaların işine de yarar.
- [ ] **Raporlama:** Projemiz ne tür raporlar sunabilir? Dünyanın en popüler fotoğraflarına sahip deniz fenerleri, en iyi fotoğraflara sahip kullanıcılar, en çok uğranılan deniz fenerlerinin olduğu ülkeler, faal olan deniz fenerleri listesi vb Bu raporlar nasıl bir uygulama baz alınabilir.
- [ ] **Entegrasyon Testleri:** Projede ilerledikçe servis ve bileşenlerin sayısı artıyor. Bu durumda entegrasyon testleri nasıl bir strateji ile iyileştirilebilir? Örneğin, **JudgeDredd** ya da **PhotoStorage** bileşenlerinin dahil olduğu vakalar için entegrasyon testleri yazılabilir ve çalışma zamanı olarak da bir TestContainer kullanılabilir.
- [ ] **Öneri Sistemi:** Platform kullanıcılarına belli kriterlere göre gidilmesi gereken ya da gidebilecekleri deniz fenerleri listeleyen bir öneri sistemi eklenebilir.

## Kontrol Listesi

Proje ve video anlatım serisi sona erdiğinde aşağıdaki sorulara cevap verebiliyor olmalıyız.

- [ ] **C#** dilinin temel özelliklerine yer verildi mi?
- [ ] **OOP** _(Object Oriented Programming)_ prensipleri uygulandı mı?
- [ ] **SOLID** prensiplerine yer verildi mi?
- [ ] Kod bazlı **teknik borç**lardan arındırıldı mı?
- [ ] Belli bir yazılım mimari stiline evrildi mi?
- [ ] Projede en az bir **Rest** tabanlı Web Api kullanıldı mı?
- [ ] Projede **gRPC** tabanlı bir servis kullanıldı mı?
- [ ] **Razor** tabanlı Web uygulaması geliştirildi mi?
- [ ] Farklı dillerde yazılmış servisler kullanıldı mı?
- [ ] Bir dağıtık sistem kurgusu tesis edildi mi?
- [ ] Dağıtık sistem kurgusu tesis edildiyse **resilience** için gerekli tedbirler alındı mı?
- [ ] Dağıtık sistem kurugusu için izleme, loglama, alarm mekanizmaları vs kullanıldı mı?
