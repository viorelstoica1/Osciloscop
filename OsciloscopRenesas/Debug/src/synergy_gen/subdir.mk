################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../src/synergy_gen/acquisition_thread.c \
../src/synergy_gen/common_data.c \
../src/synergy_gen/hal_data.c \
../src/synergy_gen/main.c \
../src/synergy_gen/pin_data.c \
../src/synergy_gen/uart_comm_thread.c 

C_DEPS += \
./src/synergy_gen/acquisition_thread.d \
./src/synergy_gen/common_data.d \
./src/synergy_gen/hal_data.d \
./src/synergy_gen/main.d \
./src/synergy_gen/pin_data.d \
./src/synergy_gen/uart_comm_thread.d 

OBJS += \
./src/synergy_gen/acquisition_thread.o \
./src/synergy_gen/common_data.o \
./src/synergy_gen/hal_data.o \
./src/synergy_gen/main.o \
./src/synergy_gen/pin_data.o \
./src/synergy_gen/uart_comm_thread.o 

SREC += \
OsciloscopRenesas.srec 

MAP += \
OsciloscopRenesas.map 


# Each subdirectory must supply rules for building sources it contributes
src/synergy_gen/%.o: ../src/synergy_gen/%.c
	$(file > $@.in,-mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O2 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -fno-strict-aliasing -Wunused -Wuninitialized -Wall -Wextra -Wmissing-declarations -Wconversion -Wpointer-arith -Wshadow -Wlogical-op -Waggregate-return -Wfloat-equal -g -D_RENESAS_SYNERGY_ -I"C:/repouri/Osciloscop/OsciloscopRenesas/synergy_cfg/ssp_cfg/bsp" -I"C:/repouri/Osciloscop/OsciloscopRenesas/synergy_cfg/ssp_cfg/driver" -I"C:/repouri/Osciloscop/OsciloscopRenesas/synergy/ssp/inc" -I"C:/repouri/Osciloscop/OsciloscopRenesas/synergy/ssp/inc/bsp" -I"C:/repouri/Osciloscop/OsciloscopRenesas/synergy/ssp/inc/bsp/cmsis/Include" -I"C:/repouri/Osciloscop/OsciloscopRenesas/synergy/ssp/inc/driver/api" -I"C:/repouri/Osciloscop/OsciloscopRenesas/synergy/ssp/inc/driver/instances" -I"C:/repouri/Osciloscop/OsciloscopRenesas/src" -I"C:/repouri/Osciloscop/OsciloscopRenesas/src/synergy_gen" -I"C:/repouri/Osciloscop/OsciloscopRenesas/synergy_cfg/ssp_cfg/framework/el" -I"C:/repouri/Osciloscop/OsciloscopRenesas/synergy/ssp/inc/framework/el" -I"C:/repouri/Osciloscop/OsciloscopRenesas/synergy/ssp/src/framework/el/tx" -std=c99 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" -x c "$<")
	@echo Building file: $< && arm-none-eabi-gcc @"$@.in"

