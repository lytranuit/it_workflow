export function getShapeName(clazz) {
    switch (clazz) {
        case 'start': return 'start-node';
        case 'end': return 'end-node';
        case 'fail': return 'fail-node';
        case 'success': return 'success-node';
        case 'gateway': return 'gateway-node';
        case 'exclusiveGateway': return 'exclusive-gateway-node';
        case 'parallelGateway': return 'parallel-gateway-node';
        case 'inclusiveGateway': return 'inclusive-gateway-node';
        case 'timerStart': return 'timer-start-node';
        case 'formTask': return 'form-task-node';
        case 'approveTask': return 'approve-task-node';
        case 'mailSystem': return 'mail-system-node';
        case 'printSystem': return 'print-system-node';
        case 'subProcess': return 'sub-process-node';
        case 'suggestTask': return 'suggest-task-node';
        default: return 'task-node';
    }
}
