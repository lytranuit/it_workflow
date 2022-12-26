import editorStyle from "../util/defaultStyle";

const taskDefaultOptions = {
    icon: null,
    iconStyle: {
        width: 12,
        height: 12,
        left: 2,
        top: 2,
    },
    style: {
        ...editorStyle.nodeStyle,
        fill: '#E7F7FE',
        stroke: '#1890FF',
        cursor: 'default',
    },
    stateStyles: {
        selected: {
            fill: '#95D6FB',
        },
        hover: {
            cursor: editorStyle.cursor.hoverNode,
        }
    }
};

const gatewayDefaultOptions = {
    icon: null,
    iconStyle: {
        width: 20,
        height: 20,
        left: 2,
        top: 2,
    },
    style: {
        ...editorStyle.nodeStyle,
        fill: '#E8FEFA',
        stroke: '#13C2C2',
        cursor: 'default',
    },
    stateStyles: {
        selected: {
            fill: '#8CE8DE',
        },
        hover: {
            cursor: editorStyle.cursor.hoverNode,
        }
    }
};

const startDefaultOptions = {
    icon: null,
    iconStyle: {
        width: 18,
        height: 18,
        left: 6,
        top: 6,
    },
    style: {
        ...editorStyle.nodeStyle,
        fill: '#FEF7E8',
        stroke: '#FA8C16',
        cursor: 'default',
    },
    stateStyles: {
        selected: {
            fill: '#FCD49A',
        },
        hover: {
            cursor: editorStyle.cursor.hoverNode,
        }
    }
};

const endDefaultOptions = {
    icon: null,
    iconStyle: {
        width: 18,
        height: 18,
        left: 6,
        top: 6,
    },
    style: {
        ...editorStyle.nodeStyle,
        fill: '#EFF7E8',
        stroke: '#F5222D',
        cursor: 'default',
    },
    stateStyles: {
        selected: {
            fill: '#CFD49A',
        },
        hover: {
            cursor: editorStyle.cursor.hoverNode,
        }
    }
};

const catchDefaultOptions = {
    icon: null,
    iconStyle: {
        width: 20,
        height: 20,
        left: -10,
        top: -8,
    },
    style: {
        ...editorStyle.nodeStyle,
        fill: '#FEF7E8',
        stroke: '#FA8C16',
        cursor: 'default',
    },
    stateStyles: {
        selected: {
            fill: '#FCD49A',
        },
        hover: {
            cursor: editorStyle.cursor.hoverNode,
        }
    }
};

export default function (G6) {
    G6.registerNode('task-node', {
        shapeType: 'rect',
        options: {
            ...taskDefaultOptions
        },
        getShapeStyle(cfg) {
            cfg.size = [80, 44];
            const width = cfg.size[0];
            const height = cfg.size[1];
            const style = {
                x: 0 - width / 2,
                y: 0 - height / 2,
                width,
                height,
                ...this.options.style,
            };
            return style;
        }
    }, 'base-node');
    G6.registerNode('form-task-node', {
        options: $.extendext(true, 'replace', {}, taskDefaultOptions, {
            icon: require('../assets/icons/flow/icon_script.svg'),
            style: {
                fill: '#FFF7E6',
                stroke: '#FFA940',
            },
            stateStyles: {
                selected: {
                    fill: '#FFE7BA',
                },
            }
        }),

    }, 'task-node');
    G6.registerNode('approve-task-node', {
        options: $.extendext(true, 'replace', {}, startDefaultOptions, {
            icon: require('../assets/icons/flow/approve.svg'),
            iconStyle: {
                width: 8,
                height: 8,
                left: 4,
                top: 4,
            },
            style: {
                fill: '#FFF7E6',
                stroke: '#FFA940',
            },
            stateStyles: {
                selected: {
                    fill: '#FFE7BA',
                },
            }

        }),
    }, 'task-node');
    G6.registerNode('suggest-task-node', {
        shapeType: 'rect',
        labelPosition: 'bottom',
        options: $.extendext(true, 'replace', {}, taskDefaultOptions, {
            icon: require('../assets/icons/flow/suggest-task.svg'),
            iconStyle: {
                width: 20,
                height: 22,
                left: 20,
                top: 10,
            },
            style: {
                fill: 'white',
                stroke: '#0000009e',
            },
            stateStyles: {
                selected: {
                    fill: '#E9E9E9',
                },
            }
        }),
        getShapeStyle(cfg) {
            cfg.size = [60, 44];
            const width = cfg.size[0];
            const height = cfg.size[1];
            const style = {
                x: 0 - width / 2,
                y: 0 - height / 2,
                width,
                height,
                ...this.options.style,
            };
            return style;
        },
    }, 'base-node');

    G6.registerNode('gateway-node', {
        shapeType: 'path',
        labelPosition: 'bottom',
        options: {
            ...gatewayDefaultOptions
        },
        getShapeStyle(cfg) {
            cfg.size = [40, 40];
            const width = cfg.size[0];
            const height = cfg.size[1];
            const gap = 4;
            const style = {
                path: [
                    ['M', 0 - gap, 0 - height / 2 + gap],
                    ['Q', 0, 0 - height / 2, gap, 0 - height / 2 + gap],
                    ['L', width / 2 - gap, 0 - gap],
                    ['Q', width / 2, 0, width / 2 - gap, gap],
                    ['L', gap, height / 2 - gap],
                    ['Q', 0, height / 2, 0 - gap, height / 2 - gap],
                    ['L', -width / 2 + gap, gap],
                    ['Q', -width / 2, 0, -width / 2 + gap, 0 - gap],
                    ['Z']
                ],
                ...this.options.style,
            };
            return style;
        },
    }, 'base-node');
    G6.registerNode('exclusive-gateway-node', {
        afterDraw(cfg, group) {
            group.icon = group.addShape('path', {
                attrs: {
                    path: [
                        ['M', -8, -8],
                        ['L', 8, 8],
                        ['Z'],
                        ['M', 8, -8],
                        ['L', -8, 8],
                        ['Z']
                    ],
                    lineWidth: 2,
                    stroke: this.options.style.stroke,
                }
            });
            this.runAnimate(cfg, group);
        },
    }, 'gateway-node');
    G6.registerNode('parallel-gateway-node', {
        afterDraw(cfg, group) {
            group.icon = group.addShape('path', {
                attrs: {
                    path: [
                        ['M', 0, -10],
                        ['L', 0, 10],
                        ['Z'],
                        ['M', -10, 0],
                        ['L', 10, 0],
                        ['Z']
                    ],
                    lineWidth: 2,
                    stroke: this.options.style.stroke,
                }
            });
            this.runAnimate(cfg, group);
        },
    }, 'gateway-node');
    G6.registerNode('inclusive-gateway-node', {
        afterDraw(cfg, group) {
            group.icon = group.addShape('circle', {
                attrs: {
                    x: 0,
                    y: 0,
                    r: 10,
                    lineWidth: 2,
                    stroke: this.options.style.stroke,
                }
            });
            this.runAnimate(cfg, group);
        },
    }, 'gateway-node');



    G6.registerNode('mail-system-node', {
        shapeType: 'rect',
        labelPosition: 'bottom',
        options: $.extendext(true, 'replace', {}, taskDefaultOptions, {
            icon: require('../assets/icons/flow/icon_mail.svg'),
            iconStyle: {
                width: 15,
                height: 15,
                left: 18,
                top: 7,
            },
            style: {
                fill: 'aliceblue',
                stroke: 'rgb(103 194 58)',
            },
            stateStyles: {
                selected: {
                    fill: 'rgb(189 221 199)',
                },
            }
        }),
        getShapeStyle(cfg) {
            cfg.size = [50, 30];
            const width = cfg.size[0];
            const height = cfg.size[1];
            const style = {
                x: 0 - width / 2,
                y: 0 - height / 2,
                width,
                height,
                ...this.options.style,
            };
            return style;
        },
    }, 'base-node');

    G6.registerNode('print-system-node', {
        shapeType: 'rect',
        labelPosition: 'bottom',
        options: $.extendext(true, 'replace', {}, taskDefaultOptions, {
            icon: require('../assets/icons/flow/printSystem.svg'),
            iconStyle: {
                width: 20,
                height: 22,
                left: 8,
                top: 9,
            },
            style: {
                fill: 'white',
                stroke: '#0000009e',
            },
            stateStyles: {
                selected: {
                    fill: '#E9E9E9',
                },
            }
        }),
        getShapeStyle(cfg) {
            cfg.size = [35, 40];
            const width = cfg.size[0];
            const height = cfg.size[1];
            const style = {
                x: 0 - width / 2,
                y: 0 - height / 2,
                width,
                height,
                ...this.options.style,
            };
            return style;
        },
    }, 'base-node');




    G6.registerNode('start-node', {
        shapeType: 'circle',
        labelPosition: 'bottom',
        options: {
            ...startDefaultOptions
        },
        getShapeStyle(cfg) {
            cfg.size = [30, 30];
            const width = cfg.size[0];
            const style = {
                x: 0,
                y: 0,
                r: width / 2,
                ...this.options.style,
            };
            if (cfg.hasOwnProperty('color')) {
                style.fill = cfg.color
            }
            return style;
        },
        afterDraw(cfg, group) {
            if (cfg.active) {
                const shape = group.get('children')[0];
                shape.animate({
                    repeat: true,
                    onFrame(ratio) {
                        const diff = ratio <= 0.5 ? ratio * 10 : (1 - ratio) * 10;
                        let radius = cfg.size;
                        if (isNaN(radius)) radius = radius[0];
                        return {
                            r: radius / 2 + diff
                        }
                    }
                }, 3000, 'easeCubic');
            }
            group.icon = group.addShape('path', {
                attrs: {
                    path: [
                        ['M', -4, -6],
                        ['L', 6, 0],
                        ['L', -4, 6],
                        ['Z'] // close
                    ],
                    fill: this.options.style.stroke,
                    stroke: this.options.style.stroke,
                }
            });
        },
        getAnchorPoints() {
            return [
                [0.5, 0], // top
                [1, 0.5], // right
                [0.5, 1], // bottom
            ]
        }
    }, 'base-node');
    G6.registerNode('fail-node', {
        shapeType: 'circle',
        labelPosition: 'bottom',
        options: {
            ...endDefaultOptions
        },
        getShapeStyle(cfg) {
            cfg.size = [30, 30];
            const width = cfg.size[0];
            const style = {
                x: 0,
                y: 0,
                r: width / 2,
                ...this.options.style,
            };
            if (cfg.hasOwnProperty('color')) {
                style.fill = cfg.color
            }
            return style;
        },
        afterDraw(cfg, group) {
            if (cfg.active) {
                const shape = group.get('children')[0];
                shape.animate({
                    repeat: true,
                    onFrame(ratio) {
                        const diff = ratio <= 0.5 ? ratio * 10 : (1 - ratio) * 10;
                        let radius = cfg.size;
                        if (isNaN(radius)) radius = radius[0];
                        return {
                            r: radius / 2 + diff
                        }
                    }
                }, 3000, 'easeCubic');
            }
            group.icon = group.addShape('rect', {
                attrs: {
                    width: 10,
                    height: 10,
                    x: -5,
                    y: -5,
                    fill: this.options.style.stroke,
                    stroke: this.options.style.stroke,
                }
            });
        },
        getAnchorPoints() {
            return [
                [0.5, 0], // top
                [0.5, 1], // bottom
                [0, 0.5], // left
            ]
        }
    }, 'base-node');
    G6.registerNode('success-node', {
        shapeType: 'circle',
        labelPosition: 'bottom',
        options: $.extendext(true, 'replace', {}, endDefaultOptions, {
            icon: require('../assets/icons/flow/success.png'),
            style: {
                fill: 'aliceblue',
                stroke: 'rgb(103 194 58)',
            },
            stateStyles: {
                selected: {
                    fill: 'rgb(189 221 199)',
                },
            }
        }),
        getShapeStyle(cfg) {
            cfg.size = [30, 30];
            const width = cfg.size[0];
            const style = {
                x: 0,
                y: 0,
                r: width / 2,
                ...this.options.style,
            };
            if (cfg.hasOwnProperty('color')) {
                style.fill = cfg.color
            }
            return style;
        },
        afterDraw(cfg, group) {
            if (cfg.active) {
                const shape = group.get('children')[0];
                shape.animate({
                    repeat: true,
                    onFrame(ratio) {
                        const diff = ratio <= 0.5 ? ratio * 10 : (1 - ratio) * 10;
                        let radius = cfg.size;
                        if (isNaN(radius)) radius = radius[0];
                        return {
                            r: radius / 2 + diff
                        }
                    }
                }, 3000, 'easeCubic');
            }
        },
        getAnchorPoints() {
            return [
                [0.5, 0], // top
                [0.5, 1], // bottom
                [0, 0.5], // left
            ]
        }
    }, 'base-node');

    //G6.registerNode('timer-start-node', {
    //    options: $.extendext(true, 'replace', {}, startDefaultOptions, { icon: require('../assets/icons/flow/icon_timer.svg') }),
    //    afterDraw(cfg, group) { this.runAnimate(cfg, group) },
    //}, 'start-node');


}
