/***********************************************************************************************************************
* File Name    : blinky_thread_entry.c
* Description  : This is a very simple example application that blinks all the LEDs on a board.
***********************************************************************************************************************/

#include "blinky_thread.h"
volatile uint32_t NumIntreruperiAprinse = 0U;
volatile uint32_t flagAdc = 0U;
uint8_t uart0_buf[17];

uint8_t CRC_calculate(uint8_t length);
void user_uart_callback(uart_callback_args_t *p_args){
    NumIntreruperiAprinse++;
}

void intrerupere_adc(adc_callback_args_t *p_args){
    flagAdc=1;
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

    volatile uint32_t delei = 1U; /*100 inseamna o secunda*/
    /*variabile UART*/
    volatile uint8_t uart0_send_num = 17U;
    volatile uint8_t uart_header_index = 0U;
    /*variabile pentru ADC*/
    volatile uint16_t valoareCititaAdc = 0U;
    volatile uint16_t adc_buf[10];
    volatile uint16_t adc_index = 0U;
    /*Initializari ADC*/
    g_adc0.p_api->open(g_adc0.p_ctrl, g_adc0.p_cfg);
    g_adc0.p_api->scanCfg(g_adc0.p_ctrl, g_adc0.p_channel_cfg);
    /*initializari UART*/
    g_uart0.p_api->open(g_uart0.p_ctrl, g_uart0.p_cfg);
    while(1){
        /*Citire de pe ADC pana are 10 valori*/
        while(adc_index < 10U){
            tx_thread_sleep(delei);
            g_adc0.p_api->scanStart(g_adc0.p_ctrl);
            while(flagAdc == 0);
            valoareCititaAdc = 0U;
            g_adc0.p_api->read(g_adc0.p_ctrl, 0, &valoareCititaAdc);
            adc_buf[adc_index] = valoareCititaAdc;
            adc_index++;
        }
        adc_index = 0U;
        /*Trimitere pe UART*/
        uart0_buf[0] = uart_header_index + 10U; /*plus 10 ca sa inceapa de la 10 headerele, la fel ca in aplicatia de pe PC*/
        uart0_buf[1] = (uint8_t)(adc_buf[0] >> 4U);
        uart0_buf[2] = (uint8_t)(((adc_buf[0] & 0x0F) << 4U) + (adc_buf[1] >> 8U));
        uart0_buf[3] = (uint8_t)(adc_buf[1] & 0xFF);
        uart0_buf[4] = (uint8_t)(adc_buf[2] >> 4U);
        uart0_buf[5] = (uint8_t)(((adc_buf[2] & 0x0F) << 4U) + (adc_buf[3] >> 8U));
        uart0_buf[6] = (uint8_t)(adc_buf[3] & 0xFF);
        uart0_buf[7] = (uint8_t)(adc_buf[4] >> 4U);
        uart0_buf[8] = (uint8_t)(((adc_buf[4] & 0x0F) << 4U) + (adc_buf[5] >> 8U));
        uart0_buf[9] = (uint8_t)(adc_buf[5] & 0xFF);
        uart0_buf[10] = (uint8_t)(adc_buf[6] >> 4U);
        uart0_buf[11] = (uint8_t)(((adc_buf[6] & 0x0F) << 4U) + (adc_buf[7] >> 8U));
        uart0_buf[12] = (uint8_t)(adc_buf[7] & 0xFF);
        uart0_buf[13] = (uint8_t)(adc_buf[8] >> 4U);
        uart0_buf[14] = (uint8_t)(((adc_buf[8] & 0x0F) << 4U) + (adc_buf[9] >> 8U));
        uart0_buf[15] = (uint8_t)(adc_buf[9] & 0xFF);
        uart0_buf[16] = CRC_calculate(17);
        g_uart0.p_api->write(g_uart0.p_ctrl, uart0_buf, uart0_send_num);
        uart_header_index = (uint8_t)((uart_header_index + 1U) % 10U);
    }
}
