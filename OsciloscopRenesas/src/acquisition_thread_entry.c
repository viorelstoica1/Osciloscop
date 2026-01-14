#include <acquisition_thread.h>

volatile uint32_t flagAdc = 0U;
volatile uint32_t flagTimer = 0U;
volatile uint16_t adc_buf[1000];
volatile uint16_t adc_index = 0U;

void intrerupere_adc(adc_callback_args_t *p_args){
    (void)p_args;
    flagAdc = 1U;
}

void intrerupere_timer(timer_callback_args_t *p_args){
    (void)p_args;
    flagTimer = 1U;
}

void acquisition_thread_entry(void)
{
    /*Initializari timer*/
    g_timer0.p_api->open(g_timer0.p_ctrl, g_timer0.p_cfg);
    /*variabile pentru ADC*/
    uint16_t valoareCititaAdc = 0U;
    /*Initializari ADC*/
    g_adc0.p_api->open(g_adc0.p_ctrl, g_adc0.p_cfg);
    g_adc0.p_api->scanCfg(g_adc0.p_ctrl, g_adc0.p_channel_cfg);
    /*initializari UART*/
    g_uart0.p_api->open(g_uart0.p_ctrl, g_uart0.p_cfg);
    while(1){
        /*Citire de pe ADC pana are 1000 valori*/
        while(adc_index < 1000U){
            g_timer0.p_api->start(g_timer0.p_ctrl);
            while(flagTimer == 0U);
            flagTimer = 0U;
            g_adc0.p_api->scanStart(g_adc0.p_ctrl);
            while(flagAdc == 0);
            valoareCititaAdc = 0U;
            g_adc0.p_api->read(g_adc0.p_ctrl, 0, &valoareCititaAdc);
            adc_buf[adc_index] = valoareCititaAdc;
            adc_index++;
        }
        adc_index = 0U;
        tx_thread_sleep(0);
    }
}
