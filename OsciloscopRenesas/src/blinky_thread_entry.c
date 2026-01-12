#include "blinky_thread.h"

#define MARIME_MESAJ_UART 18U
#define MESAJE_NECESARE_UART 100U
volatile uint32_t NumIntreruperiAprinse = 0U;
volatile uint32_t flagAdc = 0U;
volatile uint32_t flagUart = 0U;
volatile uint32_t flagTimer = 0U;
uint8_t mesaj_uart[MARIME_MESAJ_UART];
volatile uint16_t adc_offset = 0U;
volatile uint8_t uart_send_index = 0U;
volatile uint16_t adc_buf[1000];
volatile uint16_t adc_index = 0U;



uint8_t CRC_calculate(uint8_t length);
void user_uart_callback(uart_callback_args_t *p_args){
    (void)p_args;
    flagUart = 1U;
}

void intrerupere_adc(adc_callback_args_t *p_args){
    (void)p_args;
    flagAdc = 1U;
}

void intrerupere_timer(timer_callback_args_t *p_args){
    (void)p_args;
    flagTimer = 1U;
}

uint8_t CRC_calculate(uint8_t length){
    uint8_t crc=0, message[length];
    uint16_t divisor = 0x8D, dividend;
    int i, j;

    for(i=0; i<length-1; i++)
    {
        message[i] = mesaj_uart[i];
    }

    message[length-1]=0;

    dividend = (message[0] << 8) | message[1];
    for(j=15; j>=8; j--)
        if(dividend & (1 << j))
            dividend ^= divisor << (j-8);

    for(i=2; i<length; i++)
        {
            dividend = (dividend << 8) | message[i];

            for(j=15; j>=8; j--)
                if(dividend & (1 << j))
                    dividend ^= divisor << (j-8);
        }

    crc = (dividend % 256U);

    return crc;
}


void blinky_thread_entry(void)
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
        /*Trimitere pe UART*/
        for(uart_send_index = 0U; uart_send_index < MESAJE_NECESARE_UART; uart_send_index++){
            adc_offset = uart_send_index * 10U;
            mesaj_uart[0] = 10U;
            mesaj_uart[1] = uart_send_index;
            mesaj_uart[2] = (uint8_t)(adc_buf[0 + adc_offset] >> 4U);
            mesaj_uart[3] = (uint8_t)(((adc_buf[0 + adc_offset] & 0x0F) << 4U) + (adc_buf[1 + adc_offset] >> 8U));
            mesaj_uart[4] = (uint8_t)(adc_buf[1 + adc_offset] & 0xFF);
            mesaj_uart[5] = (uint8_t)(adc_buf[2 + adc_offset] >> 4U);
            mesaj_uart[6] = (uint8_t)(((adc_buf[2 + adc_offset] & 0x0F) << 4U) + (adc_buf[3 + adc_offset] >> 8U));
            mesaj_uart[7] = (uint8_t)(adc_buf[3 + adc_offset] & 0xFF);
            mesaj_uart[8] = (uint8_t)(adc_buf[4 + adc_offset] >> 4U);
            mesaj_uart[9] = (uint8_t)(((adc_buf[4 + adc_offset] & 0x0F) << 4U) + (adc_buf[5 + adc_offset] >> 8U));
            mesaj_uart[10] = (uint8_t)(adc_buf[5 + adc_offset] & 0xFF);
            mesaj_uart[11] = (uint8_t)(adc_buf[6 + adc_offset] >> 4U);
            mesaj_uart[12] = (uint8_t)(((adc_buf[6 + adc_offset] & 0x0F) << 4U) + (adc_buf[7 + adc_offset] >> 8U));
            mesaj_uart[13] = (uint8_t)(adc_buf[7 + adc_offset] & 0xFF);
            mesaj_uart[14] = (uint8_t)(adc_buf[8 + adc_offset] >> 4U);
            mesaj_uart[15] = (uint8_t)(((adc_buf[8 + adc_offset] & 0x0F) << 4U) + (adc_buf[9 + adc_offset] >> 8U));
            mesaj_uart[16] = (uint8_t)(adc_buf[9 + adc_offset] & 0xFF);
            mesaj_uart[17] = CRC_calculate(18);
            g_uart0.p_api->write(g_uart0.p_ctrl, mesaj_uart, MARIME_MESAJ_UART);
            while(flagUart == 0U);
            flagUart = 0;
        }
    }
}
