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

- [x] [Bölüm 00 - Project Lighthouse Social Açılış](https://youtu.be/xO4S60bfZPU)
- [x] [Bölüm 01 - Domain Projesinin Oluşturulması](https://youtu.be/fIsvAwxnnIQ)
- [x] [Bölüm 02 - Entity Nesnelerinin Eklenmesi](https://youtu.be/dDZHq-vI18U)
- [x] [Bölüm 03 - Enumeration Bazlı Türlerin Eklenmesi](https://youtu.be/cCW44l7fgX0)
- [x] [Bölüm 04 - Örnek Kontratların(Interface) Eklenmesi](https://youtu.be/_cta_s9zM1U)
- [x] [Bölüm 05 - Application Katmanı](https://youtu.be/SmnrE73VvUo)
- [x] [Bölüm 06 - Handler Nesnelerinin Yazılması](https://youtu.be/x6u7uMxw8qU)
- [x] [Bölüm 07 - Handler Sınıfları için Birim Testler](https://youtu.be/P_uRenWyE54)
- [x] [Bölüm 08 - DTO Nesneleri için Validator Kullanımı](https://youtu.be/MrqTqc9d2q8)
- [x] [Bölüm 09 - Teknik Borç Önlemleri için Sonarqube Kullanımı](https://youtu.be/XUiG1MwSq1o)
- [x] [Bölüm 10 - Yorumlar için Denetim Bileşeninin Eklenmesi](https://youtu.be/54GMi9i2W-4)
- [x] [Bölüm 11 - OpenAI Moderation API ile Yorum Denetim Servisinin Geliştirilmesi](https://youtu.be/PU9SqkPt41o)
- [x] [Bölüm 12 - Uçtan Uca Entegrasyon Testlerinin Yazılması](https://youtu.be/vdFRhsLcqhM)
- [x] [Bölüm 13 - PostgreSQL Veritabanı Hazırlıkları](https://youtu.be/z8G9iThiiDE)
- [x] [Bölüm 14 - Data Katmanı ve Repository Sınıflarının Geliştirilmesi](https://youtu.be/pA8xsOmsZpI)
- [x] [Bölüm 15 - Application Katmanı ve LighthouseService Bileşeninin Geliştirilmesi](https://youtu.be/ovhQM9L_hhQ)
- [x] [Bölüm 16 - Terminal Bazlı İstemci Uygulamanın Geliştirilmesi](https://youtu.be/Frbquqiq4Us)
- [x] [Bölüm 17 - MinIO Destekli PhotoStorage Bileşeninin Geliştirilmesi](https://youtu.be/RnCqWo9Bhs8)
- [x] [Bölüm 18 - İstemci Tarafında PhotoStorageService' in Kullanılması](https://youtu.be/pfPqZ1SkHdM)
- [x] [Bölüm 19 - Vault Entegrasyonu I](https://youtu.be/CN52vnOzfT4)
- [x] [Bölüm 20 - Vault Entegrasyonu II](https://youtu.be/GadLQoJNfw4)
- [x] [Bölüm 21 - Client Uygulamasını Çalışır Halde Tutmak](https://youtu.be/B8VHWE8xbZU)
- [x] [Bölüm 22 - Cache Entegrasyonu I](https://youtu.be/24CHSgqMaB4)
- [x] [Bölüm 23 - Cache Entegrasyonu II](https://youtu.be/z9t0rKR5g_8)
- [x] [Bölüm 24 - Redis Entegrasyonu](https://youtu.be/rrRxhuo8w0E)
- [x] [Bölüm 25 - Redis Servis Bileşeninin Geliştirilmesi](https://youtu.be/gRPlFqGVTCQ)
- [x] [Bölüm 26 - Pipeline Nesnelerinin İnşası](https://youtu.be/rliAGCQpGmE)
- [x] [Bölüm 27 - Pipeline Behavior Bileşenlerinin Yazılması ve Entegrasyonu](https://youtu.be/Y1uZuewmLAs)
- [x] [Bölüm 28 - Debug İşlemleri ile Pipeline Akışının İzlenmesi](https://youtu.be/p1IQqc5cD6g)
- [x] [Bölüm 29 - Lighthouse Web API Projesinin Oluşturulması](https://youtu.be/WI9lOd8uVs4)
- [x] [Bölüm 30 - Web API Projesi Debug ve Runtime Hatalarının Giderilmesi](https://youtu.be/EO1IED3soQ8)
- [x] [Bölüm 31 - Projeye Yeni Bir Özellik(Feature) Eklemek](https://youtu.be/8rV-mUXj4YQ)
- [x] [Bölüm 32 - Tüm Veriyi Döndürme! Sayfalama(Paging) Tekniğini Kullan](https://youtu.be/Il3GcRWGKmA)
- [x] [Bölüm 33 - Teknik Borçtan Kaçıl(a)maz! Sonarqube Yakalar](https://youtu.be/kajGEmY_r8M)
- [x] [Bölüm 34 - OData Servisi ile Veriyi HTTP Üzerinden Sorgulamak](https://youtu.be/FlJSnSSMMpk)
- [x] [Bölüm 35 - Peki ya DI Servislerine Bildirilen Her Bileşene İhtiyacımız Yoksa?](https://youtu.be/r34uuX8ycCQ)
- [x] [Bölüm 36 - Keycloak ile Tanışalım](https://youtu.be/rKk1iRMmXME)
- [x] [Bölüm 37 - Web API Projesinde Keycloak Bazlı Yetkilendirme(Authorization) Kullanımı](https://youtu.be/nJr1yM_Em7s)
- [x] [Bölüm 38 - Logları Elasticsearch'e Gönderelim](https://youtu.be/rP4i3yqtAP8)
- [x] [Bölüm 39 - Vault Bilgilerini Cache Üzerinde Tutmak *(Açtık Yine Dertsiz Başımıza Dert)*](https://youtu.be/t6nnL11wOnI)
- [x] [Bölüm 40 - Daha İyi Bir Hata Yönetimi *(Result Pattern Refactoring)*](https://youtu.be/2GA8q-jjNGI)
- [x] [Bölüm 41 - Gelen Bir Yorum Üzerine Sistemimize Graylog'u Adapte Ediyoruz](https://youtu.be/OmaopxCVLF8)
- [x] [Bölüm 42 - Projedeki Son Durum ve PhotoController ile Dükkana Dönüş](https://youtu.be/Ng4Yuhqruyo)
- [x] [Bölüm 43 - Photo Upload Sürecinde Dağıtık Transaction Yönetimi için SAGA Pattern](https://youtu.be/TA1CF6C4iyA)
- [x] [Bölüm 44 - SAGA Deseninin Photo Upload Sürecine Uyarlanması](https://youtu.be/i1iln8Ym96U)
- [x] [Bölüm 45 - Photo Upload Sürecinde SAGA Pattern Testleri](https://youtu.be/Q2SsV1HGa8A)
- [x] [Bölüm 46 - Event Based Sistem için Hazırlıklar](https://youtu.be/2Kdka9T9Gmo)
- [x] [Bölüm 47 - RabbitMQ için Event Publisher Bileşeninin Geliştirilmesi](https://youtu.be/XglgWP3w37M)
- [x] [Bölüm 48 - Event'ler RabbitMQ'ya Gidibilecek mi?](https://youtu.be/Ibfz3ScqgOc)
- [x] [Bölüm 49 - Nerede Bu RabbitMQ Mesajları? (Queue Oluşturma ve Kod Üzerinden Dinleme)](https://youtu.be/NzYOm4NJT5s)
- [x] [Bölüm 50 - Razor Tabanlı Backoffice Uygulaması için İlk Adımlar](https://youtu.be/h1PFUQ20g90)
- [x] [Bölüm 51 - Yeni Deniz Feneri Eklemek için Create Sayfasının Oluşturulması](https://youtu.be/CpPu6jPQW2c)
- [x] [Bölüm 52 - Deniz Fenerlerini Listeleme Sayfasının Oluşturulması](https://youtu.be/edF1Cg-cnjs)
- [x] [Bölüm 53 - Ülke Bilgilerini Çekmek için Application Katmanında Geliştirmeler](https://youtu.be/mBG8Wj74h10)
- [x] [Bölüm 54 - Razor Sayfasındaki Select Kontrolünü Servis Katmanına Bağladık](https://youtu.be/oGSCAO27UwI)

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

## Güncel Vault Bilgileri

Path : **ProjectLighthouseSocial-Dev**

```json
{
  "DbConnStr": "Host=localhost;Port=5432;Database=lighthousedb;Username=johndoe;Password=somew0rds",
  "KeycloakAudience": "account",
  "KeycloakAuthority": "http://localhost:8400/",
  "KeycloakClientId": "lighthouse-service-client",
  "KeycloakClientSecret": "Lupgay1UcpJA7vRDOr7MsrBJN5B7bJoN",
  "KeycloakClockSkew": "5",
  "KeycloakRealm": "ProjectLighthouseSocialRealm",
  "KeycloakRequireHttpsMetadata": "false",
  "KeycloakValidateAudience": "true",
  "KeycloakValidateIssuer": "true",
  "KeycloakValidateIssuerSigningKey": "true",
  "KeycloakValidateLifetime": "true",
  "MinIOAccessKey": "admin",
  "MinIOSecretKey": "password",
  "RabbitMQUser": "admin",
  "RabbitMQPassword": "admin1234"
}
```

## Redis Cache Değerlerinde Sorun Olursa

Örneğin Vault tarafında tanımlı bir secret için yeni sürüm çıktık. Eğer Cache aktifse vault değerlerinin tekrardan yüklenmesi gerekir. Bunu normalde Cache Invalidation metodunu çağırarak yapabiliriz ama her ihtimale karşı docker üzerinden de ilgili key değerini silerek ilerlemek mümkün. Örneğin vault ayarlarını silmek istersek aşağıdaki komutu kullanabiliriz.

```bash
docker exec -it plh-redis redis-cli del vault:keycloak_settings
```

## Redis Cache Testleri

Redis cache testleri sırasında docker container içerisine girip keyleri kontrol etmek veya manuel silmek isteyebiliriz. Aşağıdaki bunun için kullanabileceğimiz pratik komutlar yer alıyor.

```bash
# Redis container'ına erişim
docker exec -it plh-redis sh

# Redis CLI'yi çalıştırma
redis-cli

# Tüm anahtarları listeleme
keys *

# Key, Value ekleme, listeleme, silme
set Environment "Development"
get Environment

set user:Service "{\"user\":\"apiAccount\"}" EX 3600   # 1 saat Geçerlilik süresi
get user:Service
del user:Service

# Key var mı kontrolü
exists user:Service

# Bir key'in kalan yaşam süresini öğrenme
TTL user:Service
```

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
