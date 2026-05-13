using ClosedXML.Excel;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Export;
using CompetenceAssessment.Domain.Export.Enumerations;
using CompetenceAssessment.Domain.Export.Models;

namespace CompetenceAssessment.Application.Export;

// TODO разделить на отдельные сервисы
public class ReportExportService: IReportExportService
{
    private readonly IAssessmentAnalyticsService _analyticsService;

    public ReportExportService(IAssessmentAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    public async Task<byte[]> GenerateReportAsync(ReportRequest request, CancellationToken token = default)
    {
        return request switch
        {
            HeatmapReportRequest heatmap => await BuildHeatmapReport(heatmap, token),
            PositionMatrixReportRequest positionMatrix => await BuildPositionMatrixReport(positionMatrix, token),
            OrganizationalMaturityReportRequest maturity => await BuildOrganizationalMaturityReport(maturity, token),
            _ => throw new NotSupportedException($"Report type {request.ReportType} is not implemented yet.")
        };
    }

    private async Task<byte[]> BuildHeatmapReport(HeatmapReportRequest request, CancellationToken token)
    {
        // 1. Получение данных
        var participants = await _analyticsService.GetParticipantsCompetenciesAsync(token);

        // Сбор всех уникальных компетенций
        var competencies = participants
            .SelectMany(p => p.ParticipantCompetences.Select(c => c.Competence.Name))
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        // Фильтрация по департаменту, если задан
        if (request.DepartmentId.HasValue && request.DepartmentId.Value > 0)
        {
            participants = participants
                .Where(p => p.User.DepartmentId == request.DepartmentId.Value)
                .ToList();
        }

        // Сортировка
        IEnumerable<AssessmentParticipantCompetencies> ordered = request.SortBy switch
        {
            "avgScore" => participants.OrderByDescending(p => GetAverageScore(p)),
            _ => participants.OrderBy(p => p.User.FullName)
        };

        // 2. Создание Excel-книги
        using var workbook = new XLWorkbook();
        workbook.Properties.Title = "Тепловая карта компетенций";
        workbook.Properties.Author = "Competence Assessment System";

        // 3. Лист «Тепловая карта»
        var wsHeat = workbook.Worksheets.Add("Тепловая карта");
        BuildHeatmapSheet(wsHeat, ordered, competencies);

        // 4. Лист «Данные»
        var wsData = workbook.Worksheets.Add("Данные");
        BuildDataSheet(wsData, ordered, competencies);

        // 5. Сохранение в поток
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
    
    private async Task<byte[]> BuildPositionMatrixReport(PositionMatrixReportRequest request, CancellationToken token)
{
    // 1. Получение данных
    var positions = await _analyticsService.GetPositionsCompetenciesAsync(token);
    var participants = await _analyticsService.GetParticipantsCompetenciesAsync(token);

    // Фильтрация по должности, если указана
    PositionCompetencies? selectedPosition;
    if (request.PositionId.HasValue && request.PositionId.Value > 0)
    {
        selectedPosition = positions.FirstOrDefault(p => p.PositionId == request.PositionId.Value);
    }
    else
    {
        selectedPosition = positions.FirstOrDefault();
    }

    if (selectedPosition == null)
        throw new InvalidOperationException("Нет данных для формирования отчёта");

    // 2. Создание Excel-книги
    using var workbook = new XLWorkbook();
    workbook.Properties.Title = "Матрица компетенций по должностям";
    workbook.Properties.Author = "Competence Assessment System";

    // 3. Лист «Gap-анализ»
    var wsGap = workbook.Worksheets.Add("Gap-анализ");
    BuildGapAnalysisSheet(wsGap, selectedPosition, participants);

    // 4. Лист «Соответствие сотрудников»
    var wsCompliance = workbook.Worksheets.Add("Соответствие сотрудников");
    BuildComplianceSheet(wsCompliance, selectedPosition, participants);

    // 5. Сохранение
    using var stream = new MemoryStream();
    workbook.SaveAs(stream);
    return stream.ToArray();
}
    
    private async Task<byte[]> BuildOrganizationalMaturityReport(
    OrganizationalMaturityReportRequest request, 
    CancellationToken token)
{
    // 1. Получение данных
    var departments = await _analyticsService.GetDepartmentCompetenciesAsync(token);

    // Фильтрация по департаменту, если указан
    DepartmentCompetencies? selectedDepartment;
    if (request.DepartmentId.HasValue && request.DepartmentId.Value > 0)
    {
        selectedDepartment = departments.FirstOrDefault(d => d.DepartmentId == request.DepartmentId.Value);
    }
    else
    {
        selectedDepartment = departments.FirstOrDefault();
    }

    if (selectedDepartment == null)
        throw new InvalidOperationException("Нет данных для формирования отчёта");

    // 2. Создание Excel-книги
    using var workbook = new XLWorkbook();
    workbook.Properties.Title = "Матрица зрелости организации";
    workbook.Properties.Author = "Competence Assessment System";

    // 3. Лист «Сводка зрелости»
    var wsSummary = workbook.Worksheets.Add("Сводка зрелости");
    BuildMaturitySummarySheet(wsSummary, selectedDepartment, departments);

    // 4. Лист «Детализация»
    var wsDetails = workbook.Worksheets.Add("Детализация");
    BuildMaturityDetailsSheet(wsDetails, selectedDepartment);

    // 5. Лист «Рекомендации»
    var wsRecommendations = workbook.Worksheets.Add("Рекомендации");
    BuildRecommendationsSheet(wsRecommendations, selectedDepartment);

    // 6. Сохранение
    using var stream = new MemoryStream();
    workbook.SaveAs(stream);
    return stream.ToArray();
}

private void BuildMaturitySummarySheet(IXLWorksheet sheet,
                                       DepartmentCompetencies department,
                                       List<DepartmentCompetencies> allDepartments)
{
    sheet.Cell(1, 1).Value = $"Матрица зрелости компетенций: {department.DepartmentName}";
    sheet.Cell(1, 1).Style.Font.Bold = true;
    sheet.Cell(1, 1).Style.Font.FontSize = 14;
    sheet.Range(1, 1, 1, 5).Merge();
    
    sheet.Cell(3, 1).Value = "Департамент:";
    sheet.Cell(3, 1).Style.Font.Bold = true;
    sheet.Cell(3, 2).Value = department.DepartmentName;

    sheet.Cell(4, 1).Value = "Количество сотрудников:";
    sheet.Cell(4, 1).Style.Font.Bold = true;
    sheet.Cell(4, 2).Value = department.Employees.Count;

    sheet.Cell(5, 1).Value = "Общий процент зрелости:";
    sheet.Cell(5, 1).Style.Font.Bold = true;
    sheet.Cell(5, 2).Value = Math.Round(department.Percentage, 1);
    sheet.Cell(5, 2).Style.Font.Bold = true;
    
    int row = 7;
    sheet.Cell(row, 1).Value = "Уровень зрелости:";
    sheet.Cell(row, 1).Style.Font.Bold = true;
    sheet.Cell(row, 1).Style.Font.FontSize = 12;

    var (levelName, levelColor, levelDescription) = GetMaturityLevelInfo(department.Percentage);
    
    sheet.Cell(row, 2).Value = levelName;
    sheet.Cell(row, 2).Style.Font.Bold = true;
    sheet.Cell(row, 2).Style.Font.FontSize = 14;
    sheet.Cell(row, 2).Style.Font.FontColor = XLColor.FromHtml(levelColor);

    row = 9;
    sheet.Cell(row, 1).Value = "Описание уровня:";
    sheet.Cell(row, 1).Style.Font.Bold = true;
    sheet.Cell(row, 2).Value = levelDescription;
    sheet.Cell(row, 2).Style.Alignment.WrapText = true;
    sheet.Range(row, 2, row, 5).Merge();
    
    row = 12;
    sheet.Cell(row, 1).Value = "Прогресс зрелости:";
    sheet.Cell(row, 1).Style.Font.Bold = true;
    
    row = 13;
    int progressBars = (int)(department.Percentage / 10); // 0-10 полосок
    string progressBar = new string('█', progressBars) + new string('░', 10 - progressBars);
    sheet.Cell(row, 1).Value = progressBar;
    sheet.Cell(row, 1).Style.Font.FontName = "Consolas";
    sheet.Cell(row, 1).Style.Font.FontSize = 12;
    sheet.Cell(row, 2).Value = $"{Math.Round(department.Percentage, 0)}%";
    sheet.Cell(row, 2).Style.Font.Bold = true;
    sheet.Cell(row, 2).Style.Font.FontColor = XLColor.FromHtml(levelColor);
    
    row = 14;
    var maturityStages = new[] { "0%", "25%", "50%", "75%", "100%" };
    for (int i = 0; i < maturityStages.Length; i++)
    {
        sheet.Cell(row, 1 + i).Value = maturityStages[i];
        sheet.Cell(row, 1 + i).Style.Font.FontSize = 8;
        sheet.Cell(row, 1 + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    }
    
    row = 17;
    sheet.Cell(row, 1).Value = "Сравнение департаментов:";
    sheet.Cell(row, 1).Style.Font.Bold = true;
    sheet.Cell(row, 1).Style.Font.FontSize = 12;

    row = 18;
    sheet.Cell(row, 1).Value = "Департамент";
    sheet.Cell(row, 2).Value = "Уровень зрелости (%)";
    sheet.Cell(row, 3).Value = "Уровень";
    sheet.Cell(row, 4).Value = "Прогресс";

    var headerRange = sheet.Range(row, 1, row, 4);
    headerRange.Style.Font.Bold = true;
    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#f4f4f4");
    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

    row = 19;
    foreach (var dept in allDepartments.OrderByDescending(d => d.Percentage))
    {
        bool isCurrent = dept.DepartmentId == department.DepartmentId;
        
        sheet.Cell(row, 1).Value = dept.DepartmentName;
        sheet.Cell(row, 2).Value = Math.Round(dept.Percentage, 1);
        sheet.Cell(row, 3).Value = GetMaturityLevelInfo(dept.Percentage).Name;
        
        int bars = (int)(dept.Percentage / 10);
        string bar = new string('█', bars) + new string('░', 10 - bars);
        sheet.Cell(row, 4).Value = bar;
        sheet.Cell(row, 4).Style.Font.FontName = "Consolas";
        
        sheet.Cell(row, 2).Style.Font.FontColor = XLColor.FromHtml(
            GetMaturityLevelInfo(dept.Percentage).Color);
        
        if (isCurrent)
        {
            sheet.Range(row, 1, row, 4).Style.Font.Bold = true;
            sheet.Range(row, 1, row, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#e8f4ff");
        }
        
        row++;
    }

    int lastComparisonRow = row - 1;
    
    if (lastComparisonRow >= 19)
    {
        var percentageRange = sheet.Range(19, 2, lastComparisonRow, 2);
        percentageRange.AddConditionalFormat().ColorScale()
            .LowestValue(XLColor.FromHtml("#da1e28"))
            .HighestValue(XLColor.FromHtml("#198038"));
    }
    
    var comparisonTableRange = sheet.Range(18, 1, lastComparisonRow, 4);
    comparisonTableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    comparisonTableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    
    sheet.Columns().AdjustToContents();
    sheet.Column(2).Width = 30;
    sheet.Column(4).Width = 15;
}

private void BuildMaturityDetailsSheet(IXLWorksheet sheet, 
                                       DepartmentCompetencies department)
{
    sheet.Cell(1, 1).Value = $"Детализация зрелости компетенций: {department.DepartmentName}";
    sheet.Cell(1, 1).Style.Font.Bold = true;
    sheet.Cell(1, 1).Style.Font.FontSize = 14;
    sheet.Range(1, 1, 1, 5).Merge();
    
    int headerRow = 3;
    sheet.Cell(headerRow, 1).Value = "Компетенция";
    sheet.Cell(headerRow, 2).Value = "Уровень зрелости (%)";
    sheet.Cell(headerRow, 3).Value = "Уровень";
    sheet.Cell(headerRow, 4).Value = "Прогресс";
    sheet.Cell(headerRow, 5).Value = "Статус";

    var headerRange = sheet.Range(headerRow, 1, headerRow, 5);
    headerRange.Style.Font.Bold = true;
    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#f4f4f4");
    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

    int row = headerRow + 1;
    var sortedCompetencies = department.Competencies
        .OrderBy(c => c.Percentage)
        .ToList();

    foreach (var competence in sortedCompetencies)
    {
        sheet.Cell(row, 1).Value = competence.Competence.Name;
        sheet.Cell(row, 1).Style.Font.Bold = true;
        
        sheet.Cell(row, 2).Value = Math.Round(competence.Percentage, 1);
        sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        sheet.Cell(row, 2).Style.Font.Bold = true;
        
        var (levelName, levelColor, _) = GetMaturityLevelInfo(competence.Percentage);
        sheet.Cell(row, 3).Value = levelName;
        sheet.Cell(row, 3).Style.Font.FontColor = XLColor.FromHtml(levelColor);
        sheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        
        int bars = (int)(competence.Percentage / 10);
        string progressBar = new string('█', bars) + new string('░', 10 - bars);
        sheet.Cell(row, 4).Value = progressBar;
        sheet.Cell(row, 4).Style.Font.FontName = "Consolas";
        sheet.Cell(row, 4).Style.Font.FontColor = XLColor.FromHtml(levelColor);
        sheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        
        var statusCell = sheet.Cell(row, 5);
        if (competence.Percentage >= 80)
        {
            statusCell.Value = "✓ Высокий";
            statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#e8f5e9");
            statusCell.Style.Font.FontColor = XLColor.FromHtml("#198038");
        }
        else if (competence.Percentage >= 60)
        {
            statusCell.Value = "⚠ Средний";
            statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#fff3e0");
            statusCell.Style.Font.FontColor = XLColor.FromHtml("#e65100");
        }
        else if (competence.Percentage >= 40)
        {
            statusCell.Value = "⚠ Ниже среднего";
            statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#fff8e1");
            statusCell.Style.Font.FontColor = XLColor.FromHtml("#f57f17");
        }
        else
        {
            statusCell.Value = "✗ Низкий";
            statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#ffebee");
            statusCell.Style.Font.FontColor = XLColor.Red;
        }
        statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        statusCell.Style.Font.Bold = true;

        row++;
    }

    int lastRow = row - 1;
    
    if (lastRow >= headerRow + 1)
    {
        var percentageRange = sheet.Range(headerRow + 1, 2, lastRow, 2);
        percentageRange.AddConditionalFormat().ColorScale()
            .LowestValue(XLColor.FromHtml("#da1e28"))
            .HighestValue(XLColor.FromHtml("#198038"));
    }
    
    var dataRange = sheet.Range(headerRow, 1, lastRow, 5);
    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    
    dataRange.SetAutoFilter();
    
    int statsRow = lastRow + 3;
    sheet.Cell(statsRow, 1).Value = "Статистика по компетенциям:";
    sheet.Cell(statsRow, 1).Style.Font.Bold = true;
    sheet.Cell(statsRow, 1).Style.Font.FontSize = 12;

    statsRow++;
    var highCount = sortedCompetencies.Count(c => c.Percentage >= 80);
    var mediumCount = sortedCompetencies.Count(c => c.Percentage >= 60 && c.Percentage < 80);
    var lowCount = sortedCompetencies.Count(c => c.Percentage < 60);

    sheet.Cell(statsRow, 1).Value = "Высокий уровень (≥80%):";
    sheet.Cell(statsRow, 2).Value = highCount;
    sheet.Cell(statsRow, 2).Style.Font.FontColor = XLColor.FromHtml("#198038");
    sheet.Cell(statsRow, 2).Style.Font.Bold = true;
    statsRow++;

    sheet.Cell(statsRow, 1).Value = "Средний уровень (60-79%):";
    sheet.Cell(statsRow, 2).Value = mediumCount;
    sheet.Cell(statsRow, 2).Style.Font.FontColor = XLColor.FromHtml("#e65100");
    sheet.Cell(statsRow, 2).Style.Font.Bold = true;
    statsRow++;

    sheet.Cell(statsRow, 1).Value = "Требует развития (<60%):";
    sheet.Cell(statsRow, 2).Value = lowCount;
    sheet.Cell(statsRow, 2).Style.Font.FontColor = XLColor.Red;
    sheet.Cell(statsRow, 2).Style.Font.Bold = true;
    
    sheet.Columns().AdjustToContents();
    sheet.Column(1).Width = 40;
    sheet.Column(4).Width = 20;
    sheet.SheetView.Freeze(headerRow, 1);
}

private void BuildRecommendationsSheet(IXLWorksheet sheet, 
                                       DepartmentCompetencies department)
{
    sheet.Cell(1, 1).Value = $"Рекомендации по повышению зрелости: {department.DepartmentName}";
    sheet.Cell(1, 1).Style.Font.Bold = true;
    sheet.Cell(1, 1).Style.Font.FontSize = 14;
    sheet.Range(1, 1, 1, 3).Merge();
    
    sheet.Cell(3, 1).Value = "Общая рекомендация:";
    sheet.Cell(3, 1).Style.Font.Bold = true;
    sheet.Cell(3, 1).Style.Font.FontSize = 12;

    string generalRecommendation = GetGeneralRecommendation(department.Percentage);
    sheet.Cell(3, 2).Value = generalRecommendation;
    sheet.Cell(3, 2).Style.Alignment.WrapText = true;
    sheet.Range(3, 2, 5, 3).Merge();
    
    int row = 7;
    sheet.Cell(row, 1).Value = "Компетенции, требующие развития:";
    sheet.Cell(row, 1).Style.Font.Bold = true;
    sheet.Cell(row, 1).Style.Font.FontSize = 12;

    var problematicCompetencies = department.Competencies
        .Where(c => c.Percentage < 60)
        .OrderBy(c => c.Percentage)
        .ToList();

    if (problematicCompetencies.Any())
    {
        row = 8;
        sheet.Cell(row, 1).Value = "Компетенция";
        sheet.Cell(row, 2).Value = "Текущий уровень";
        sheet.Cell(row, 3).Value = "Рекомендация";

        var headerRange = sheet.Range(row, 1, row, 3);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#f4f4f4");

        row = 9;
        foreach (var comp in problematicCompetencies)
        {
            sheet.Cell(row, 1).Value = comp.Competence.Name;
            sheet.Cell(row, 1).Style.Font.Bold = true;
            
            int bars = (int)(comp.Percentage / 10);
            string progressBar = new string('█', bars) + new string('░', 10 - bars);
            sheet.Cell(row, 2).Value = $"{Math.Round(comp.Percentage, 1)}% {progressBar}";
            sheet.Cell(row, 2).Style.Font.FontColor = XLColor.Red;
            sheet.Cell(row, 2).Style.Font.Bold = true;
            sheet.Cell(row, 2).Style.Font.FontName = "Consolas";

            string recommendation = GetCompetenceRecommendation(comp.Percentage);
            sheet.Cell(row, 3).Value = recommendation;
            sheet.Cell(row, 3).Style.Alignment.WrapText = true;
            
            row++;
        }

        int lastRow = row - 1;
        
        var tableRange = sheet.Range(8, 1, lastRow, 3);
        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    }
    else
    {
        row = 8;
        sheet.Cell(row, 1).Value = "✓ Все компетенции на хорошем уровне";
        sheet.Cell(row, 1).Style.Font.FontColor = XLColor.FromHtml("#198038");
        sheet.Cell(row, 1).Style.Font.Bold = true;
    }
    
    row += 2;
    sheet.Cell(row, 1).Value = "Сильные стороны:";
    sheet.Cell(row, 1).Style.Font.Bold = true;
    sheet.Cell(row, 1).Style.Font.FontSize = 12;

    var strongCompetencies = department.Competencies
        .Where(c => c.Percentage >= 80)
        .OrderByDescending(c => c.Percentage)
        .Take(5)
        .ToList();

    row++;
    if (strongCompetencies.Any())
    {
        foreach (var comp in strongCompetencies)
        {
            sheet.Cell(row, 1).Value = $"✓ {comp.Competence.Name}";
            sheet.Cell(row, 1).Style.Font.FontColor = XLColor.FromHtml("#198038");
            sheet.Cell(row, 2).Value = $"{Math.Round(comp.Percentage, 1)}%";
            sheet.Cell(row, 2).Style.Font.FontColor = XLColor.FromHtml("#198038");
            sheet.Cell(row, 2).Style.Font.Bold = true;
            row++;
        }
    }
    else
    {
        sheet.Cell(row, 1).Value = "Нет компетенций с высоким уровнем зрелости";
        sheet.Cell(row, 1).Style.Font.FontColor = XLColor.FromHtml("#6f6f6f");
    }
    
    row += 2;
    sheet.Cell(row, 1).Value = "Рекомендуемый план действий:";
    sheet.Cell(row, 1).Style.Font.Bold = true;
    sheet.Cell(row, 1).Style.Font.FontSize = 12;

    row++;
    var actionPlan = GetActionPlan(department.Percentage);
    foreach (var action in actionPlan)
    {
        sheet.Cell(row, 1).Value = action.Item1;
        sheet.Cell(row, 1).Style.Font.Bold = true;
        sheet.Cell(row, 2).Value = action.Item2;
        sheet.Cell(row, 2).Style.Alignment.WrapText = true;
        row++;
    }
    
    sheet.SheetView.Freeze(1, 1);
    
    sheet.Columns().AdjustToContents();
    sheet.Column(1).Width = 35;
    sheet.Column(2).Width = 50;
    sheet.Column(3).Width = 60;
}

private (string Name, string Color, string Description) GetMaturityLevelInfo(decimal percentage)
{
    if (percentage >= 90)
        return ("Оптимизирующий", "#198038", 
                "Процессы постоянно улучшаются на основе количественных данных. " +
                "Внедрены инновации и лучшие практики.");
    if (percentage >= 70)
        return ("Количественно управляемый", "#0f62ac", 
                "Процессы измеряются и контролируются с использованием статистических " +
                "и количественных методов. Принимаются решения на основе данных.");
    if (percentage >= 50)
        return ("Определенный", "#ff832b", 
                "Процессы стандартизированы, документированы и интегрированы " +
                "в организационную систему. Персонал обучен и следует процессам.");
    if (percentage >= 30)
        return ("Управляемый", "#f1c21b", 
                "Процессы планируются и отслеживаются, но могут быть непоследовательными. " +
                "Базовые практики управления проектами установлены.");
    
    return ("Начальный", "#da1e28", 
            "Процессы непредсказуемы, слабо контролируемы и реактивны. " +
            "Успех зависит от индивидуальных усилий, а не от системы.");
}

private string GetGeneralRecommendation(decimal percentage)
{
    if (percentage >= 90)
        return "Поддерживать текущий уровень зрелости, продолжать внедрение инноваций " +
               "и распространять лучшие практики на другие подразделения.";
    if (percentage >= 70)
        return "Сосредоточиться на внедрении системы непрерывного улучшения " +
               "и развитии культуры инноваций.";
    if (percentage >= 50)
        return "Разработать систему KPI для измерения эффективности процессов " +
               "и внедрить инструменты количественного анализа.";
    if (percentage >= 30)
        return "Стандартизировать ключевые процессы, разработать документацию " +
               "и провести обучение сотрудников.";
    
    return "Начать с внедрения базовых практик управления: планирование, " +
           "мониторинг и контроль. Назначить ответственных за ключевые процессы.";
}

private string GetCompetenceRecommendation(decimal percentage)
{
    if (percentage >= 60)
        return "Достигнут средний уровень. Рекомендуется внедрить регулярную оценку " +
               "и систему непрерывного улучшения.";
    if (percentage >= 40)
        return "Необходимо разработать программу развития компетенции, " +
               "провести обучение и внедрить контрольные метрики.";
    if (percentage >= 20)
        return "Критически низкий уровень. Требуется срочная программа развития: " +
               "обучение, менторство, регулярная оценка прогресса.";
    
    return "Компетенция практически отсутствует. Необходима комплексная программа " +
           "развития с нуля: найм специалистов, обучение, внедрение процессов.";
}

private List<(string Priority, string Action)> GetActionPlan(decimal percentage)
{
    var actions = new List<(string, string)>();
    
    if (percentage < 40)
    {
        actions.Add(("🔴 Критично:", "Определить ключевые компетенции и ответственных лиц"));
        actions.Add(("🔴 Критично:", "Разработать базовые регламенты и процедуры"));
        actions.Add(("🟡 Срочно:", "Провести первичное обучение персонала"));
        actions.Add(("🟡 Срочно:", "Внедрить систему оценки и мониторинга"));
    }
    else if (percentage < 60)
    {
        actions.Add(("🟡 Важно:", "Стандартизировать процессы и документировать процедуры"));
        actions.Add(("🟡 Важно:", "Разработать программу повышения квалификации"));
        actions.Add(("🟢 Планово:", "Внедрить KPI для ключевых компетенций"));
    }
    else if (percentage < 80)
    {
        actions.Add(("🟢 Планово:", "Внедрить систему количественного анализа"));
        actions.Add(("🟢 Планово:", "Разработать программу непрерывного улучшения"));
        actions.Add(("🟢 Планово:", "Масштабировать лучшие практики"));
    }
    else
    {
        actions.Add(("🟢 Поддержка:", "Поддерживать текущий уровень зрелости"));
        actions.Add(("🟢 Инновации:", "Внедрять передовые методологии и технологии"));
        actions.Add(("🟢 Обмен:", "Распространять лучшие практики на другие подразделения"));
    }
    
    return actions;
}

private void BuildGapAnalysisSheet(IXLWorksheet sheet, 
                                   PositionCompetencies position,
                                   List<AssessmentParticipantCompetencies> participants)
{
    sheet.Cell(1, 1).Value = $"Gap-анализ компетенций для должности: {position.PositionName}";
    sheet.Cell(1, 1).Style.Font.Bold = true;
    sheet.Cell(1, 1).Style.Font.FontSize = 14;
    sheet.Range(1, 1, 1, 5).Merge();
    
    sheet.Cell(2, 1).Value = "Количество сотрудников:";
    sheet.Cell(2, 2).Value = position.Employees.Count;
    sheet.Cell(3, 1).Value = "Общий уровень соответствия:";
    sheet.Cell(3, 2).Value = $"{Math.Round(position.Percentage, 1)}%";
    sheet.Cell(3, 2).Style.Font.Bold = true;
    
    int headerRow = 5;
    sheet.Cell(headerRow, 1).Value = "Компетенция";
    sheet.Cell(headerRow, 2).Value = "Требуемый уровень (эталон)";
    sheet.Cell(headerRow, 3).Value = "Средний текущий уровень";
    sheet.Cell(headerRow, 4).Value = "Разрыв (%)";
    sheet.Cell(headerRow, 5).Value = "Статус";

    var headerRange = sheet.Range(headerRow, 1, headerRow, 5);
    headerRange.Style.Font.Bold = true;
    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#f4f4f4");
    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    
    int row = headerRow + 1;
    foreach (var competence in position.Competencies)
    {
        sheet.Cell(row, 1).Value = competence.Competence.Name;
        sheet.Cell(row, 1).Style.Font.Bold = true;
        
        decimal requiredLevel = 40m;
        sheet.Cell(row, 2).Value = $"{requiredLevel}%";
        sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        
        decimal currentLevel = competence.Percentage;
        sheet.Cell(row, 3).Value = $"{Math.Round(currentLevel, 1)}%";
        sheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        sheet.Cell(row, 3).Style.Font.FontColor = XLColor.FromHtml(GetLevelColor(currentLevel));
        
        decimal gap = requiredLevel - currentLevel;
        var gapCell = sheet.Cell(row, 4);
        gapCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        
        if (gap > 0)
        {
            gapCell.Style.Font.FontColor = XLColor.Red;
            gapCell.Value = $"-{Math.Round(gap, 1)}%";
        }
        else
        {
            gapCell.Style.Font.FontColor = XLColor.FromHtml("#198038");
            gapCell.Value = $"+{Math.Round(Math.Abs(gap), 1)}%";
        }

        // Статус
        var statusCell = sheet.Cell(row, 5);
        if (gap <= 0)
        {
            statusCell.Value = "✓ Соответствует";
            statusCell.Style.Font.FontColor = XLColor.FromHtml("#198038");
        }
        else if (gap <= 10)
        {
            statusCell.Value = "⚠ Близко к норме";
            statusCell.Style.Font.FontColor = XLColor.FromHtml("#f1c21b");
        }
        else
        {
            statusCell.Value = "✗ Требует развития";
            statusCell.Style.Font.FontColor = XLColor.Red;
        }

        row++;
    }

    int lastRow = row - 1;

    // ColorScale для текущих уровней
    var currentLevelRange = sheet.Range(headerRow + 1, 3, lastRow, 3);
    currentLevelRange.AddConditionalFormat().ColorScale()
        .LowestValue(XLColor.FromHtml("#da1e28"))
        .HighestValue(XLColor.FromHtml("#0f62ac"));

    // Границы таблицы
    var dataRange = sheet.Range(headerRow, 1, lastRow, 5);
    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

    // Данные для диаграммы
    // if (position.Competencies.Count > 0)
    // {
    //     int chartDataStartRow = lastRow + 3;
    //     
    //     // Заголовки данных для диаграммы
    //     sheet.Cell(chartDataStartRow, 1).Value = "Компетенция";
    //     sheet.Cell(chartDataStartRow, 2).Value = "Требуемый";
    //     sheet.Cell(chartDataStartRow, 3).Value = "Текущий";
    //
    //     // Данные
    //     int chartRow = chartDataStartRow + 1;
    //     foreach (var competence in position.Competencies)
    //     {
    //         sheet.Cell(chartRow, 1).Value = TruncateString(competence.Competence.Name, 25);
    //         sheet.Cell(chartRow, 2).Value = 40m; // требуемый уровень
    //         sheet.Cell(chartRow, 3).Value = Math.Round(competence.Percentage, 1);
    //         chartRow++;
    //     }
    //
    //     int chartLastRow = chartRow - 1;

        // // Создаём столбчатую диаграмму
        // var chart = sheet.Charts.AddBarChart(); // <-- правильный метод
        // chart.Title = $"Gap-анализ: {position.PositionName}";
        //
        // // Устанавливаем позицию и размер
        // chart.SetPosition(chartDataStartRow + 1, 0, 5, 0);
        // chart.SetSize(800, 400);
        //
        // // Добавляем серии
        // chart.AddSeries("Требуемый уровень", 
        //     sheet.Range(chartDataStartRow + 1, 2, chartLastRow, 2));
        // chart.AddSeries("Текущий уровень", 
        //     sheet.Range(chartDataStartRow + 1, 3, chartLastRow, 3));
        //
        // // Устанавливаем категории (подписи оси X)
        // chart.SetCategoryLabels(sheet.Range(chartDataStartRow + 1, 1, chartLastRow, 1));
        //
        // // Настройка осей
        // chart.SetYAxis("Процент выполнения (%)");
    // }

    sheet.Columns().AdjustToContents();
    sheet.SheetView.Freeze(headerRow, 1);
}

private void BuildComplianceSheet(IXLWorksheet sheet,
                                  PositionCompetencies position,
                                  List<AssessmentParticipantCompetencies> participants)
{
    // Заголовок
    sheet.Cell(1, 1).Value = $"Соответствие сотрудников требованиям: {position.PositionName}";
    sheet.Cell(1, 1).Style.Font.Bold = true;
    sheet.Cell(1, 1).Style.Font.FontSize = 14;

    var competencies = position.Competencies.Select(c => c.Competence).ToList();
    
    // Заголовки таблицы
    int headerRow = 3;
    int col = 1;
    sheet.Cell(headerRow, col++).Value = "Сотрудник";
    sheet.Cell(headerRow, col++).Value = "Должность";
    
    foreach (var comp in competencies)
    {
        sheet.Cell(headerRow, col++).Value = comp.Name;
    }
    sheet.Cell(headerRow, col).Value = "Общий статус";
    
    var headerRange = sheet.Range(headerRow, 1, headerRow, competencies.Count + 3);
    headerRange.Style.Font.Bold = true;
    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#f4f4f4");
    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    headerRange.Style.Alignment.WrapText = true;

    // Получаем сотрудников на этой должности
    var employeesOnPosition = position.Employees.ToList();

    int row = headerRow + 1;
    foreach (var employee in employeesOnPosition)
    {
        col = 1;
        
        // Имя сотрудника
        sheet.Cell(row, col++).Value = employee.FullName;
        sheet.Cell(row, col).Style.Font.Bold = true;
        
        // Должность
        sheet.Cell(row, col++).Value = employee.Position;

        // Проверка по каждой компетенции
        bool allCompliant = true;
        var participantData = participants.FirstOrDefault(p => p.User.Id == employee.Id);

        foreach (var comp in competencies)
        {
            var cell = sheet.Cell(row, col);
            
            if (participantData != null)
            {
                var participantComp = participantData.ParticipantCompetences
                    .FirstOrDefault(c => c.Competence.Id == comp.Id);
                
                if (participantComp != null)
                {
                    decimal currentScore = participantComp.Percentage;
                    bool meetsRequirement = currentScore >= 40; // порог 40%

                    cell.Value = $" {Math.Round(currentScore, 0)}%";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    if (meetsRequirement)
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#e8f5e9"); // зелёный фон
                        cell.Style.Font.FontColor = XLColor.FromHtml("#198038");
                    }
                    else
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#ffebee"); // красный фон
                        cell.Style.Font.FontColor = XLColor.Red;
                        allCompliant = false;
                    }
                }
                else
                {
                    cell.Value = "—";
                    cell.Style.Font.FontColor = XLColor.FromHtml("#8d8d8d");
                    allCompliant = false;
                }
            }
            else
            {
                cell.Value = "Н/Д";
                cell.Style.Font.FontColor = XLColor.FromHtml("#8d8d8d");
                allCompliant = false;
            }
            
            col++;
        }

        // Общий статус сотрудника
        var statusCell = sheet.Cell(row, col);
        if (allCompliant)
        {
            statusCell.Value = "✓ Соответствует";
            statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#198038");
            statusCell.Style.Font.FontColor = XLColor.White;
        }
        else
        {
            statusCell.Value = "✗ Требует внимания";
            statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#da1e28");
            statusCell.Style.Font.FontColor = XLColor.White;
        }
        statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        statusCell.Style.Font.Bold = true;

        row++;
    }

    int lastRow = row - 1;

    // Статистика соответствия
    if (lastRow >= headerRow + 1)
    {
        int statsRow = lastRow + 2;
        int compliantCount = employeesOnPosition.Count(e =>
        {
            var participantData = participants.FirstOrDefault(p => p.User.Id == e.Id);
            if (participantData == null) return false;
            
            return competencies.All(comp =>
            {
                var compData = participantData.ParticipantCompetences
                    .FirstOrDefault(c => c.Competence.Id == comp.Id);
                return compData != null && compData.Percentage >= 40;
            });
        });

        sheet.Cell(statsRow, 1).Value = "Статистика соответствия:";
        sheet.Cell(statsRow, 1).Style.Font.Bold = true;
        
        sheet.Cell(statsRow + 1, 1).Value = "Соответствуют требованиям:";
        sheet.Cell(statsRow + 1, 2).Value = $"{compliantCount} из {employeesOnPosition.Count}";
        sheet.Cell(statsRow + 1, 2).Style.Font.Bold = true;
        
        decimal complianceRate = employeesOnPosition.Count > 0 
            ? (decimal)compliantCount / employeesOnPosition.Count * 100 
            : 0;
        sheet.Cell(statsRow + 2, 1).Value = "Процент соответствия:";
        sheet.Cell(statsRow + 2, 2).Value = $"{Math.Round(complianceRate, 1)}%";
        sheet.Cell(statsRow + 2, 2).Style.Font.Bold = true;
        sheet.Cell(statsRow + 2, 2).Style.Font.FontColor = XLColor.FromHtml(
            complianceRate >= 80 ? "#198038" : complianceRate >= 60 ? "#f1c21b" : "#da1e28");
    }

    // Границы
    var dataRange = sheet.Range(headerRow, 1, lastRow, competencies.Count + 3);
    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

    // Автофильтр
    dataRange.SetAutoFilter();
    
    // Закрепление
    sheet.SheetView.Freeze(headerRow, 2);
    sheet.Columns().AdjustToContents();
}

private void BuildComplianceSummary(IXLWorksheet sheet, 
                                    PositionCompetencies position,
                                    int startRow,
                                    List<AssessmentParticipantCompetencies> participants)
{
    // Этот метод можно использовать для сводной статистики
    sheet.Cell(startRow, 1).Value = "Сводка по должности";
    sheet.Cell(startRow, 1).Style.Font.Bold = true;
    sheet.Cell(startRow, 1).Style.Font.FontSize = 12;

    sheet.Cell(startRow + 1, 1).Value = "Должность:";
    sheet.Cell(startRow + 1, 2).Value = position.PositionName;
    
    sheet.Cell(startRow + 2, 1).Value = "Уровень соответствия:";
    sheet.Cell(startRow + 2, 2).Value = $"{Math.Round(position.Percentage, 1)}%";
    
    sheet.Cell(startRow + 3, 1).Value = "Уровень:";
    sheet.Cell(startRow + 3, 2).Value = position.Level;

    // Топ-3 проблемных компетенций
    var problematicComps = position.Competencies
        .OrderBy(c => c.Percentage)
        .Take(3)
        .ToList();

    if (problematicComps.Any())
    {
        sheet.Cell(startRow + 5, 1).Value = "Требуют развития:";
        sheet.Cell(startRow + 5, 1).Style.Font.Bold = true;
        
        int row = startRow + 6;
        foreach (var comp in problematicComps)
        {
            sheet.Cell(row, 1).Value = comp.Competence.Name;
            sheet.Cell(row, 2).Value = $"{Math.Round(comp.Percentage, 1)}%";
            row++;
        }
    }
}

    private string TruncateString(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }

    private decimal GetAverageScore(AssessmentParticipantCompetencies participant)
    {
        var comps = participant.ParticipantCompetences;
        if (comps.Count == 0) return 0;
        return comps.Average(c => c.Percentage);
    }

private void BuildHeatmapSheet(IXLWorksheet sheet,
                               IEnumerable<AssessmentParticipantCompetencies> participants,
                               List<string> competencies)
{
    int col = 1;
    sheet.Cell(1, col++).Value = "Сотрудник/Должность";
    foreach (var comp in competencies)
        sheet.Cell(1, col++).Value = comp;
    sheet.Cell(1, col).Value = "Средний балл";

    // Стиль заголовков
    var headerRange = sheet.Range(1, 1, 1, competencies.Count + 2);
    headerRange.Style.Font.Bold = true;
    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#f4f4f4");

    int row = 2;
    foreach (var p in participants)
    {
        col = 1;
        sheet.Cell(row, col++).Value = $"{p.User.FullName}\n{p.User.Position}";

        foreach (var compName in competencies)
        {
            var competence = p.ParticipantCompetences
                .FirstOrDefault(c => c.Competence.Name == compName);
            if (competence != null)
            {
                var cell = sheet.Cell(row, col);
                cell.Value = Math.Round(competence.Percentage, 0);
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml(GetLevelColor(competence.Percentage));
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            else
            {
                sheet.Cell(row, col).Value = "—";
            }
            col++;
        }

        // Средний балл
        var avgCell = sheet.Cell(row, col);
        avgCell.Value = Math.Round(GetAverageScore(p), 0);
        avgCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        avgCell.Style.Font.Bold = true;

        row++;
    }

    // Вычисляем последнюю строку с данными
    int lastRow = row - 1;
    int avgColumn = competencies.Count + 2;

    // Проверяем, что есть данные (хотя бы одна строка кроме заголовка)
    if (lastRow >= 2)
    {
        // Применяем ColorScale ко всему столбцу средних баллов ОДИН РАЗ
        var avgRange = sheet.Range(2, avgColumn, lastRow, avgColumn);
        avgRange.AddConditionalFormat().ColorScale()
            .LowestValue(XLColor.FromHtml("#da1e28"))
            .HighestValue(XLColor.FromHtml("#0f62ac"));

        // Автофильтр
        var usedRange = sheet.Range(1, 1, lastRow, competencies.Count + 2);
        usedRange.SetAutoFilter();
    }

    // Оформление
    sheet.Columns().AdjustToContents();
    sheet.SheetView.Freeze(1, 1);
}
private void BuildDataSheet(IXLWorksheet sheet,
    IEnumerable<AssessmentParticipantCompetencies> participants,
    List<string> competencies)
{
    // Заголовки
    sheet.Cell(1, 1).Value = "EmployeeId";
    sheet.Cell(1, 2).Value = "Department";
    sheet.Cell(1, 3).Value = "FullName";
    sheet.Cell(1, 4).Value = "Position";
    sheet.Cell(1, 5).Value = "Competence";
    sheet.Cell(1, 6).Value = "Reference";
    sheet.Cell(1, 7).Value = "Received";
    sheet.Cell(1, 8).Value = "Percentage";

    // Стиль заголовков
    var headerRange = sheet.Range(1, 1, 1, 8);
    headerRange.Style.Font.Bold = true;
    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#f4f4f4");

    int row = 2;
    foreach (var p in participants)
    {
        foreach (var comp in p.ParticipantCompetences)
        {
            sheet.Cell(row, 1).Value = p.User.Id;
            sheet.Cell(row, 2).Value = p.User.Department;
            sheet.Cell(row, 3).Value = p.User.FullName;
            sheet.Cell(row, 4).Value = p.User.Position;
            sheet.Cell(row, 5).Value = comp.Competence.Name;
            sheet.Cell(row, 6).Value = comp.Scale;
            sheet.Cell(row, 7).Value = comp.Score;
            sheet.Cell(row, 8).Value = Math.Round(comp.Percentage, 2);
            row++;
        }
    }

    // Проверка на наличие данных
    if (row > 2)
    {
        int lastRow = row - 1;
        var dataRange = sheet.Range(1, 1, lastRow, 8);
        
        // Создаём таблицу — автофильтр включится автоматически
        var table = dataRange.CreateTable();
        table.Theme = XLTableTheme.TableStyleMedium2;
        
        // Повторный вызов SetAutoFilter() НЕ НУЖЕН — таблица уже содержит автофильтр
    }

    sheet.Columns().AdjustToContents();
}
    private string GetLevelColor(decimal percentage)
    {
        if (percentage >= 80) return "#0f62ac";
        if (percentage >= 60) return "#ff832b";
        if (percentage >= 40) return "#f1c21b";
        return "#da1e28";
    }
}