# MVVM 리팩토링 가이드

## 🔴 Critical Issues (즉시 수정 필요)

### 1. BoardViewModel.cs - 무한 재귀 버그
**위치**: Line 36
```csharp
public int LeftCellToOpenCount
{
    get => LeftCellToOpenCount;  // ❌ 무한 재귀 발생!
    set => ...
}
```
**문제**: getter가 자기 자신을 반환하여 스택 오버플로우 발생
**수정**: `get => leftCellToOpenCount;`로 변경

---

### 2. BoardViewModel.cs - PropertyChanged 누락
**위치**: Line 176
```csharp
leftCellToOpenCount--;
```
**문제**: 값 변경 시 PropertyChanged 이벤트가 발생하지 않음
**수정**: `LeftCellToOpenCount--;`로 변경하여 Property setter를 통해 변경

---

### 3. BoardViewModel.cs - 내부 구현 노출 (MVVM 위반)
**위치**: Line 13-14
```csharp
public Board board { get; private set; }  // ❌ View에서 직접 접근
public CellViewModel[,] Cells { get; private set; }  // ❌ 2D 배열 노출
```
**문제**: 
- View에서 `boardViewModel.board.rows`, `boardViewModel.board.cols` 직접 접근
- ViewModel의 내부 구현이 View에 노출됨

**수정 방법**:
```csharp
private Board board;  // private으로 변경
private CellViewModel[,] cells;  // private으로 변경

// 필요한 속성만 노출
public int Rows => board.rows;
public int Cols => board.cols;
public CellViewModel GetCell(int row, int col) => cells[row, col];
```

---

### 4. CellViewModel.cs - Model 직접 노출 (MVVM 위반)
**위치**: Line 6
```csharp
public Cell cell;  // ❌ Model이 직접 노출됨
```
**문제**: View에서 `cellViewModel.cell.value`, `cellViewModel.cell.row` 등 직접 접근
**수정 방법**:
```csharp
private Cell cell;  // private으로 변경

// 필요한 속성만 노출
public int Value => cell.value;
public int Row => cell.row;
public int Col => cell.col;
```

---

### 5. CellButton.cs - View에서 Model 직접 접근 (MVVM 위반)
**위치**: Line 63, 74-75, 90, 95
```csharp
var cell = cellViewModel.cell;  // ❌ Model 직접 접근
if (cell.isRevealed) ...
cell.value ...
cell.row ...
```
**문제**: View가 ViewModel을 거치지 않고 Model에 직접 접근
**수정**: ViewModel의 Property를 통해 접근

---

### 6. GamePage.cs - View에서 ViewModel 내부 구현 접근 (MVVM 위반)
**위치**: Line 41-42, 52-53, 127, 136, 173
```csharp
boardViewModel.board.rows  // ❌ 내부 구현 노출
boardViewModel.board.cols  // ❌ 내부 구현 노출
```
**문제**: View가 ViewModel의 내부 구조를 알고 있음
**수정**: ViewModel에 `Rows`, `Cols` 속성 추가 후 사용

---

## 🟡 Medium Issues (구조 개선)

### 7. Namespace 누락
**위치**: 
- `CellViewModel.cs` (Line 4) - namespace 없음
- `CellButton.cs` (Line 9) - namespace 없음
- `InfoBar.cs` (Line 9) - namespace 없음

**수정**: 적절한 namespace 추가

---

### 8. Models - 필드 캡슐화
**위치**: `Cell.cs`, `Board.cs`
```csharp
public int value;  // ❌ public 필드
public bool isRevealed;  // ❌ public 필드
```
**문제**: public 필드 사용으로 캡슐화 위반
**수정**: Property로 변경 (필요시)

---

### 9. CellViewModel - Value 속성 누락
**문제**: `CellViewModel`에 `Value` 속성이 없어 View에서 `cell.value`로 직접 접근
**수정**: `CellViewModel`에 `Value` Property 추가

---

### 10. BoardViewModel - 2D 배열 대신 Collection 사용 고려
**위치**: Line 14
```csharp
public CellViewModel[,] Cells { get; private set; }  // 2D 배열
```
**문제**: 2D 배열은 데이터 바인딩에 불리함
**개선 제안**: `ObservableCollection<ObservableCollection<CellViewModel>>` 또는 Flatten된 Collection 사용

---

## 🟢 Low Priority Issues (코드 품질)

### 11. 명명 규칙
- `board` → `Board` (PascalCase)
- `cell` → `Cell` (PascalCase)
- `isRevealed` → `IsRevealed` (C# 네이밍 컨벤션)

---

### 12. 불필요한 using 문
**위치**: `BoardViewModel.cs` Line 5
```csharp
using Tizen.Network.Nfc;  // ❌ 사용되지 않음
```

---

### 13. 주석 처리된 코드
**위치**: `GamePage.cs` Line 29-32
```csharp
// AppBar = new AppBar()  // 삭제 또는 구현
```

---

### 14. TODO 주석 처리
**위치**: 
- `BoardViewModel.cs` Line 168: `// TODO : BFS로 바꾸기`
- `GamePage.cs` Line 198: `// TODO: 'ImagePaths.Firework'...`

---

## 📋 리팩토링 우선순위

1. **즉시 수정** (Critical):
   - [ ] LeftCellToOpenCount getter 버그 수정
   - [ ] PropertyChanged 이벤트 누락 수정
   - [ ] ViewModel 내부 구현 노출 제거
   - [ ] View에서 Model 직접 접근 제거

2. **빠른 개선** (Medium):
   - [ ] Namespace 추가
   - [ ] CellViewModel에 Value 속성 추가
   - [ ] View에서 ViewModel 내부 구현 접근 제거

3. **점진적 개선** (Low):
   - [ ] 명명 규칙 통일
   - [ ] 불필요한 코드 정리
   - [ ] TODO 주석 처리

