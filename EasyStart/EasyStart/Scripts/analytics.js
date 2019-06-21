const URLAnalytics = "/Analytics/GetReport";
const ReportType = {
    Unknown: 0,
    CountOrder: 1,
    Top5Categories: 2,
    Top5Products: 3,
    DeliveryMethod: 4,
    NewUsers: 5,
    Revenue: 6
}

class BaseReport {
    constructor(containerId, branchId, reportType, urlAnalytics) {
        this.nameReport = null;
        this.chart = null;
        this.branchId = branchId;
        this.reportType = reportType;
        this.dateTo = new Date();
        this.dateFrom = new Date(this.dateTo);
        this.urlAnalytics = urlAnalytics;
        this.filter = {
            BranchId: this.branchId,
            DateFrom: this.dateFrom.toGMTString(),
            DateTo: this.dateTo.toGMTString(),
            ReportType: this.reportType
        }

        this.$containerId = $(`#${containerId}`)
        this.chartId = `_${Math.random().toString(36).substr(2, 9)}`
        this.perioId = `period${this.chartId}`
        this.$chartItem = $(`
            <div class="chart-item">
                <div class="chart-container">
                    <canvas id='${this.chartId}'> </canvas>
                </div>
                <div class="chart-footer">
                    <div class='period'>
                        <span class="period-description">
                            Период:
                        </span>
                        <div class="group">
                            <input required="" id="${this.perioId}">
                            <i class="fal fa-calendar-alt"></i>
                        </div>
                    </div>
                </div>
            </div>`)

        this.$containerId.append(this.$chartItem);
        this.canvas = this.$chartItem.find(`#${this.chartId}`)[0];
        this.context = this.canvas.getContext('2d');
    }

    initReport() {
        let loader = new Loader(this.$chartItem)
        loader.start()

        this.loadReportData(loader);
    }

    isElementInViewport(el) {
        let rect = el.getBoundingClientRect();
        let fitsLeft = (rect.left >= 0 && rect.left <= $(window).width());
        let fitsTop = (rect.top >= 0 && rect.top <= $(window).height());
        let fitsRight = (rect.right >= 0 && rect.right <= $(window).width());
        let fitsBottom = (rect.bottom >= 0 && rect.bottom <= $(window).height());

        return {
            top: fitsTop,
            left: fitsLeft,
            right: fitsRight,
            bottom: fitsBottom,
            all: (fitsLeft && fitsTop && fitsRight && fitsBottom)
        };
}

    initPeriod() {
        let options = {
            position: "bottom center",
            range: true,
            multipleDatesSeparator: " - ",
            toggleSelected: false,
            onHide: (dp, animationCompleted) => {
                dp.update("position", "bottom center");

                if (!dp.maxRange && !animationCompleted) {
                    dp.selectDate(dp.minRange);
                }

                if (!animationCompleted) {
                    this.dateFrom = dp.minRange;
                    this.dateTo = dp.maxRange;
                    this.filter.DateFrom = this.dateFrom.toGMTString();
                    this.filter.DateTo = this.dateTo.toGMTString()
                    this.initReport();
                }
            },
            onShow: (inst, animationComplete) => {
                if (!animationComplete) {
                    var iFits = false;
                    $.each(["bottom center", "top center", "right center", "right bottom", "right top"], (i, pos) => {
                        if (!iFits) {
                            inst.update("position", pos);
                            var fits = this.isElementInViewport(inst.$datepicker[0]);
                            if (fits.all) {
                                iFits = true;
                            }
                        }
                    });
                }
            }
        };
        let $inputDate = $(`#${this.perioId}`);
        $inputDate.datepicker(options);
        let datePicker = $inputDate.data("datepicker");

        datePicker.selectDate([this.dateFrom, this.dateTo]);
        $inputDate.next("i").bind("click", function () {
            datePicker.show();
        })
    }

    loadReportData(loader) {
        const successFunc = (reportData) => {
            let labels = reportData.Success ? reportData.Data.Labels : [];
            let data = reportData.Success ? reportData.Data.Data : [];

            this.renderChart(labels, data)

            if (loader) {
                loader.stop();
            }
        }

        $.post(this.urlAnalytics, this.filter, successCallBack(successFunc, null));
    }

    prerenderProcessingData(labels, data) {
        throw new TypeError("Method prerenderProcessingData not implemented");
    }

    renderChart(labels, data) {
        if (this.chart != null) {
            this.chart.destroy();
        }

        let dataObj = this.prerenderProcessingData(labels, data);
        let option = this.getOptionForChart(dataObj.labels, dataObj.data)
        this.chart = new Chart(this.context, option);
    }

    /**
     * Implement in the class heir
     * */
    getOptionForChart() {
        throw new TypeError("Method getOptionForChart not implemented");
    }

    init() {
        this.initPeriod();
        this.initReport();
    }
}

class CountBaseReport extends BaseReport {
    constructor(containerId, branchId, reportType, urlAnalytics) {
        super(containerId, branchId, reportType, urlAnalytics);

        this.dateFrom.setDate(this.dateFrom.getDate() - 6);
        this.filter.DateFrom = this.dateFrom.toGMTString();
    }

    prerenderProcessingData(labels, data) {
        return {
            labels,
            data
        };
    }

    getOptionForChart(labels, data) {
        return {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: this.nameReport,
                    data: data,
                    backgroundColor: 'rgba(0, 0, 0, 0)',
                    borderColor: 'rgb(105, 180, 238)',
                    borderWidth: 2
                }]
            },
            options: {
                legend: {
                    display: false,
                    labels: {
                        fontSize: 10
                    }
                },
                title: {
                    display: true,
                    text: this.nameReport,
                    fontSize: 18
                },
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                }
            }
        }
    }
}

class CountOrderReport extends CountBaseReport {
    constructor(containerId, branchId, urlAnalytics) {
        super(containerId, branchId, ReportType.CountOrder, urlAnalytics);

        this.nameReport = 'Количество заказов';

        this.init();
    }
}

class RevenueReport extends CountBaseReport {
    constructor(containerId, branchId, urlAnalytics) {
        super(containerId, branchId, ReportType.Revenue, urlAnalytics);

        this.nameReport = 'Выручка';
        this.dateFrom = new Date(this.dateTo);
        this.dateFrom.setDate(this.dateFrom.getDate() - 13);
        this.filter.DateFrom = this.dateFrom.toGMTString();

        this.init();
    }
}

class NewUsersReport extends CountBaseReport {
    constructor(containerId, branchId, urlAnalytics) {
        super(containerId, branchId, ReportType.NewUsers, urlAnalytics);

        this.nameReport = 'Количество новых пользователей';
        this.dateTo = new Date();
        this.dateFrom = new Date(this.dateTo);
        this.dateFrom.setMonth(this.dateFrom.getMonth() - 3);
        this.dateFrom.setDate(1);
        this.filter.DateFrom = this.dateFrom.toGMTString();

        this.init();
    }
}

class TopBaseReport extends BaseReport {
    constructor(containerId, branchId, reportType, urlAnalytics) {
        super(containerId, branchId, reportType, urlAnalytics);

        this.dateFrom.setDate(this.dateFrom.getDate() - 13);
        this.filter.DateFrom = this.dateFrom.toGMTString();

        this.backgroundColor = [
            'rgba(255, 99, 132, 0.5)',
            'rgba(54, 162, 235, 0.5)',
            'rgba(255, 206, 86, 0.5)',
            'rgba(75, 192, 192, 0.5)',
            'rgba(153, 102, 255, 0.5)',
        ];
        this.borderColor = [
            'rgba(255, 99, 132, 1)',
            'rgba(54, 162, 235, 1)',
            'rgba(255, 206, 86, 1)',
            'rgba(75, 192, 192, 1)',
            'rgba(153, 102, 255, 1)',
        ];
    }

    prerenderProcessingData(labels, data) {
        let result = {
            labels: labels,
            data: []
        }

        for (let i = 0; i < labels.length; ++i) {
            let newDataItem = {
                label: labels[i],
                data: [data[i]],
                backgroundColor: this.backgroundColor[i],
                borderColor: this.borderColor[i],
                borderWidth: 1
            }

            result.data[i] = newDataItem;
        }

        return result;
    }

    getOptionForChart(labels, data) {
        return {
            type: 'bar',
            data: {
                labels: [''],
                datasets: data
            },
            options: {
                legend: {
                    display: true,
                    labels: {
                        fontSize: 10
                    }
                },
                title: {
                    display: true,
                    text: this.nameReport,
                    fontSize: 18
                }
            }
        }
    }
}

class Top5Categories extends TopBaseReport {
    constructor(containerId, branchId, urlAnalytics) {
        super(containerId, branchId, ReportType.Top5Categories, urlAnalytics);

        this.nameReport = 'Топ 5 категорий';

        this.init();
    }
}

class Top5Products extends TopBaseReport {
    constructor(containerId, branchId, urlAnalytics) {
        super(containerId, branchId, ReportType.Top5Products, urlAnalytics);

        this.nameReport = 'Топ 5 продуктов';

        this.init();
    }
}

class DeliveryMethod extends BaseReport {
    constructor(containerId, branchId, urlAnalytics) {
        super(containerId, branchId, ReportType.DeliveryMethod, urlAnalytics);

        this.nameReport = 'Способ получения товара (%)';
        this.dateFrom.setDate(this.dateFrom.getDate() - 13);
        this.filter.DateFrom = this.dateFrom.toGMTString();

        this.init();
    }

    prerenderProcessingData(labels, data) {
        return {
            labels,
            data
        };
    }

    getOptionForChart(labels, data) {
        return {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    label: this.nameReport,
                    data: data,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.7)',
                        'rgba(153, 102, 255, 0.7)',

                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(153, 102, 255, 1)',

                    ],
                    borderWidth: 1
                }]
            },
            options: {
                legend: {
                    display: true,
                    labels: {
                        fontSize: 10
                    }
                },
                title: {
                    display: true,
                    text: this.nameReport,
                    fontSize: 18
                },
            }
        }
    }
}