/***********************************************************************************************************************
* File Name    : blinky_thread_entry.c
* Description  : This is a very simple example application that blinks all the LEDs on a board.
***********************************************************************************************************************/

#include "blinky_thread.h"
volatile uint32_t flagUart = 0U;
volatile uint32_t flagAdc = 0U;
volatile uint32_t flagTimer = 0U;
#define UART_MESSAGE_SIZE 18U
uint8_t uart0_buf[UART_MESSAGE_SIZE];
#define ADC_BUF_SIZE 1000U
#define UART_SEND_MESSAGES 100U

uint8_t CRC_calculate(uint8_t length);
void user_uart_callback(uart_callback_args_t *p_args){
    (void)p_args;
    flagUart=1U;
}

void intrerupere_adc(adc_callback_args_t *p_args){
    (void)p_args;
    flagAdc=1;
}

void intrerupere_timer(timer_callback_args_t *p_args){
    (void)p_args;
    flagTimer=1;
}

uint8_t CRC_calculate(uint8_t length){
    uint8_t crc=0, message[length];
    uint16_t divisor = 0x8D, dividend;
    int i, j;

    for(i=0; i<length-1; i++)
    {
        message[i] = uart0_buf[i];
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

/*******************************************************************************************************************//**
 * @brief  Blinky example application
 *
 * Blinks all leds at a rate of 1 second using the the threadx sleep function.
 * Only references two other modules including the BSP, IOPORT.
 *
 **********************************************************************************************************************/
void blinky_thread_entry(void)
{
    /*variabile pentru ADC*/
    uint16_t adc_buf[ADC_BUF_SIZE];
    uint16_t valoareCititaAdc;
    /*initializari ELC*/
    //g_elc.p_api->linkSet(ELC_PERIPHERAL_ADC0, ELC_EVENT_GPT0_COUNTER_OVERFLOW);
    //g_elc.p_api->init();
    //g_elc.p_api->enable();
    /*Initializari timer*/
    g_timer0.p_api->open(g_timer0.p_ctrl, g_timer0.p_cfg);
    /*Initializari ADC*/
    g_adc0.p_api->open(g_adc0.p_ctrl, g_adc0.p_cfg);
    g_adc0.p_api->scanCfg(g_adc0.p_ctrl, g_adc0.p_channel_cfg);
    /*initializari UART*/
    g_uart0.p_api->open(g_uart0.p_ctrl, g_uart0.p_cfg);
    while(1){
        /*Citire de pe ADC pana are ADC_BUF_SIZE valori*/
        for(uint16_t adc_index = 0U; adc_index < ADC_BUF_SIZE; adc_index++){
            g_timer0.p_api->start(g_timer0.p_ctrl);
            while(flagTimer == 0U);
            flagTimer = 0U;
            g_adc0.p_api->scanStart(g_adc0.p_ctrl);
            while(flagAdc == 0);
            g_adc0.p_api->read(g_adc0.p_ctrl, 0, &valoareCititaAdc);
            adc_buf[adc_index] = valoareCititaAdc;
        }
        /*Trimitere pe UART*/
        for(uint8_t uart_send_index = 0U; uart_send_index < UART_SEND_MESSAGES; uart_send_index++){
            uint16_t adc_offset = uart_send_index * 10U;
            uart0_buf[0] = 10U;
            uart0_buf[1] = uart_send_index;
            uart0_buf[2] = (uint8_t)(adc_buf[0 + adc_offset] >> 4U);
            uart0_buf[3] = (uint8_t)(((adc_buf[0 + adc_offset] & 0x0F) << 4U) + (adc_buf[1 + adc_offset] >> 8U));
            uart0_buf[4] = (uint8_t)(adc_buf[1 + adc_offset] & 0xFF);
            uart0_buf[5] = (uint8_t)(adc_buf[2 + adc_offset] >> 4U);
            uart0_buf[6] = (uint8_t)(((adc_buf[2 + adc_offset] & 0x0F) << 4U) + (adc_buf[3 + adc_offset] >> 8U));
            uart0_buf[7] = (uint8_t)(adc_buf[3 + adc_offset] & 0xFF);
            uart0_buf[8] = (uint8_t)(adc_buf[4 + adc_offset] >> 4U);
            uart0_buf[9] = (uint8_t)(((adc_buf[4 + adc_offset] & 0x0F) << 4U) + (adc_buf[5 + adc_offset] >> 8U));
            uart0_buf[10] = (uint8_t)(adc_buf[5 + adc_offset] & 0xFF);
            uart0_buf[11] = (uint8_t)(adc_buf[6 + adc_offset] >> 4U);
            uart0_buf[12] = (uint8_t)(((adc_buf[6 + adc_offset] & 0x0F) << 4U) + (adc_buf[7 + adc_offset] >> 8U));
            uart0_buf[13] = (uint8_t)(adc_buf[7 + adc_offset] & 0xFF);
            uart0_buf[14] = (uint8_t)(adc_buf[8 + adc_offset] >> 4U);
            uart0_buf[15] = (uint8_t)(((adc_buf[8 + adc_offset] & 0x0F) << 4U) + (adc_buf[9 + adc_offset] >> 8U));
            uart0_buf[16] = (uint8_t)(adc_buf[9 + adc_offset] & 0xFF);
            uart0_buf[17] = CRC_calculate(18);
            g_uart0.p_api->write(g_uart0.p_ctrl, uart0_buf, UART_MESSAGE_SIZE);
            while(flagUart == 0U);
            flagUart = 0;
        }
        //tx_thread_sleep(0);
    }
}
