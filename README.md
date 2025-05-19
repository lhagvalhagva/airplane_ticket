# Агаарын тээврийн систем (Air Ticket Management System)

## Товч танилцуулга

Энэхүү систем нь нислэгийн зорчигчдын бүртгэл, суудал захиалга, тасалбар үүсгэх зэрэг үйлдлийг хийх боломжийг олгоно.

## API жишээнүүд

### Өгөгдлийн санг эхлүүлэх

```bash
curl -X POST http://localhost:5027/api/dbinitializer/initialize
```

### Нислэгүүд харах

```bash
curl -X GET http://localhost:5027/api/flights
```

### Зорчигчид харах

```bash
curl -X GET http://localhost:5027/api/passengers
```

### Суудлаас Тасалбар үүсгэх (Boarding Pass)

#### 1. Суудлын дугаараар тасалбар үүсгэх (SeatNumber)

```bash
curl -X POST http://localhost:5027/api/boardingpasses \
  -H "Content-Type: application/json" \
  -d '{"FlightId": 1, "PassengerId": 1, "SeatNumber": "1A"}'
```

#### 2. Суудлын ID-аар тасалбар үүсгэх (SeatId)

```bash
curl -X POST http://localhost:5027/api/boardingpasses \
  -H "Content-Type: application/json" \
  -d '{"FlightId": 1, "PassengerId": 2, "SeatId": 10}'
```

### Тасалбарыг ID-аар харах

```bash
curl -X GET http://localhost:5027/api/boardingpasses/1
```

### Тодорхой нислэгийн бүх тасалбарыг харах

```bash
curl -X GET "http://localhost:5027/api/boardingpasses?flightId=1"
```

### Зорчигчийг нислэгт бүртгэх

```bash
curl -X POST http://localhost:5027/api/flightpassengers \
  -H "Content-Type: application/json" \
  -d '{"FlightId": 1, "PassengerId": 3}'
```

### Нислэгийн төлөв өөрчлөх

```bash
curl -X PUT http://localhost:5027/api/flights/1/status \
  -H "Content-Type: application/json" \
  -d '{"Status": 2}'
```

## Төлөвийн кодууд

### FlightStatus тоон утгууд:

- 0: CheckingIn (Бүртгэл хийгдэж байгаа)
- 1: GateClosed (Хаалга хаагдсан)
- 2: InAir (Нислэгт явж байгаа)
- 3: Delayed (Хойшилсон)
- 4: Landed (Газардсан)
- 5: Cancelled (Цуцлагдсан)
